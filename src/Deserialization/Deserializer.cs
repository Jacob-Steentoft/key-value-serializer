using System.Buffers;
using System.IO.Pipelines;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Cache;

namespace KeyValueSerializer.Deserialization;

internal static class Deserializer
{
    public static async ValueTask<T> DeserializeStreamAsync<T>(Stream stream, KeyValueCache cache,
        KeyValueConfiguration config, CancellationToken cancellationToken)
        where T : new()
    {
        var buildObject = new T();
        Guard.IsNotNull(buildObject, "Cannot instantiate null as an object");

        if (stream.Length <= 0)
        {
            return buildObject;
        }

        var pipeReader = PipeReader.Create(stream);
        KeyValueProperty? property = null;
        try
        {
            while (true)
            {
                var result = await pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                var buffer = result.Buffer;

                var sequencePosition = ProcessBuffer(buildObject, ref property, buffer, cache, config);

                if (result.IsCompleted)
                {
                    break;
                }

                pipeReader.AdvanceTo(sequencePosition, buffer.End);
            }
        }
        finally
        {
            await pipeReader.CompleteAsync();
        }

        return buildObject;
    }

    private static SequencePosition ProcessBuffer(object buildObject, ref KeyValueProperty? property,
        ReadOnlySequence<byte> sequence, KeyValueCache properties, KeyValueConfiguration config)
    {
        var reader = new SequenceReader<byte>(sequence);

        long consumedBytes;
        do
        {
            consumedBytes = reader.Consumed;
            reader.AdvancePastAny(config.SkipFiller);

            // Skip comments
            if (reader.IsNext(config.CommentStart))
            {
                if (!reader.TryAdvanceToAny(config.CommentEnd))
                {
                    break;
                }

                continue;
            }

            // Get the object property name
            if (property is null)
            {
                if (!reader.TryReadTo(out ReadOnlySpan<byte> propertySpan, config.ValueStart, false))
                {
                    break;
                }

                reader.Advance(1);
                var propertyName = propertySpan.TrimEnd(config.WhiteSpaces);

                property = properties.GetKeyValueProperty(propertyName);
            }

            // Set the object property value
            if (!TryAssignProperty(buildObject, property, config, ref reader))
            {
                break;
            }

            property = null;
        } while (!reader.End && reader.Consumed > consumedBytes);

        return reader.Position;
    }

    private static bool TryAssignProperty(object buildObject, KeyValueProperty property,
        KeyValueConfiguration config, ref SequenceReader<byte> reader)
    {
        reader.AdvancePastAny(config.WhiteSpaces);

        if (!reader.TryPeek(out var byteValue))
        {
            return false;
        }

        if (byteValue == config.ArrayStart)
        {
            return TryAssignArray(buildObject, property, config, ref reader);
        }

        if (byteValue == config.StringSeparator)
        {
            return TryAssignString(buildObject, property, config, ref reader);
        }

        return TryAssignValue(buildObject, property, config, ref reader);
    }

    private static bool TryAssignValue(object buildObject, KeyValueProperty property, KeyValueConfiguration options,
        ref SequenceReader<byte> reader)
    {
        if (!reader.TryReadTo(out ReadOnlySpan<byte> valueBytes, options.ValueEnd, false))
        {
            return false;
        }

        var trimmedValue = valueBytes.TrimEnd(options.WhiteSpaces);

        ValueParser.SetProperty(buildObject, property, trimmedValue);
        return true;
    }

    private static bool TryAssignString(object buildObject, KeyValueProperty property, KeyValueConfiguration options,
        ref SequenceReader<byte> reader)
    {
        // Advance and rewind to prevent ending the read after the first character
        reader.Advance(1);
        if (!reader.TryReadTo(out ReadOnlySpan<byte> stringBytes, options.StringSeparator, options.StringIgnoreCharacter))
        {
            reader.Rewind(1);
            return false;
        }

        ValueParser.SetProperty(buildObject, property, stringBytes);
        return true;
    }

    private static bool TryAssignArray(object buildObject, KeyValueProperty property, KeyValueConfiguration options,
        ref SequenceReader<byte> reader)
    {
        var arrayEndIndex = IndexOfArrayEnd(reader.UnreadSpan, options, out var arraySize);
        if (arrayEndIndex == -1)
        {
            return false;
        }

        var remainingArray = reader.UnreadSpan.Slice(0, arrayEndIndex);

        ValueParser.SetArrayProperty(buildObject, property, remainingArray, options, arraySize);
        reader.Advance(arrayEndIndex + 1);
        return true;
    }

    private static int IndexOfArrayEnd(scoped ReadOnlySpan<byte> buffer, KeyValueConfiguration options,
        out int arraySize)
    {
        arraySize = 1;
        const int invalidLength = -1;
        for (var index = 0; index < buffer.Length; index++)
        {
            var bufferByte = buffer[index];

            if (bufferByte == options.StringSeparator)
            {
                var stringIndex = index;
                do
                {
                    stringIndex = buffer.Slice(stringIndex + 1).IndexOf(options.StringSeparator);
                    if (stringIndex == -1)
                    {
                        return invalidLength;
                    }
                } while (buffer[stringIndex - 1] == options.StringIgnoreCharacter);

                index += stringIndex + 1;
                continue;
            }

            if (bufferByte == options.ArraySeparator)
            {
                arraySize++;
                continue;
            }

            if (bufferByte == options.ArrayEnd)
            {
                return index + 1;
            }
        }

        return invalidLength;
    }
}
