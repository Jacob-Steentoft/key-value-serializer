using System.Buffers;
using System.IO.Pipelines;
using System.Text;
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

                if (!properties.TryGetKeyValueProperty(propertyName, out property!))
                {
                    ThrowHelper.ThrowInvalidOperationException(
                        $"The key '{Encoding.UTF8.GetString(propertyName)}' was not found in the type");
                }
            }

            // Set the object property value
            if (!TryAssignProperty(ref reader, buildObject, property, config))
            {
                break;
            }

            property = null;
        } while (!reader.End && reader.Consumed > consumedBytes);

        return reader.Position;
    }

    private static bool TryAssignProperty(ref SequenceReader<byte> reader, object buildObject,
        KeyValueProperty property, KeyValueConfiguration config)
    {
        reader.AdvancePastAny(config.WhiteSpaces);

        if (!reader.TryPeek(out var byteValue))
        {
            return false;
        }

        if (byteValue == config.ArrayStart)
        {
            return TryAssignArray(ref reader, buildObject, property, config);
        }

        if (byteValue == config.StringSeparator)
        {
            return TryAssignString(ref reader, buildObject, property, config);
        }

        return TryAssignValue(ref reader, buildObject, property, config);
    }

    private static bool TryAssignValue(ref SequenceReader<byte> reader, object buildObject, KeyValueProperty property,
        KeyValueConfiguration config)
    {
        if (!reader.TryReadTo(out ReadOnlySpan<byte> valueBytes, config.ValueEnd, false))
        {
            return false;
        }

        var trimmedValue = valueBytes.TrimEnd(config.WhiteSpaces);

        ValueParser.SetProperty(buildObject, property, trimmedValue, config);
        return true;
    }

    private static bool TryAssignString(ref SequenceReader<byte> reader, object buildObject, KeyValueProperty property,
        KeyValueConfiguration config)
    {
        reader.Advance(1);
        if (!reader.TryReadTo(out ReadOnlySpan<byte> stringBytes, config.StringSeparator, config.StringIgnoreCharacter))
        {
            reader.Rewind(1);
            return false;
        }

        ValueParser.SetProperty(buildObject, property, stringBytes, config);
        return true;
    }

    private static bool TryAssignArray(ref SequenceReader<byte> reader, object buildObject, KeyValueProperty property,
        KeyValueConfiguration config)
    {
        var arrayEndIndex = ValueParser.IndexOfArrayEnd(reader.UnreadSpan, config, out var arraySize);
        if (arrayEndIndex == -1)
        {
            return false;
        }

        var remainingArray = reader.UnreadSpan.Slice(0, arrayEndIndex);

        ValueParser.SetArrayProperty(buildObject, property, remainingArray, config, arraySize);
        reader.Advance(arrayEndIndex + 1);
        return true;
    }
}
