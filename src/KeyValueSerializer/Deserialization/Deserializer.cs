using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Deserialization;

internal static class Deserializer
{
    public static async ValueTask<T> DeserializeStreamAsync<T>(Stream stream, KeyValueCache cache,
        KeyValueConfiguration config, CancellationToken cancellationToken)
        where T : new()
    {
        object buildObject = new T();
        Guard.IsNotNull(buildObject, "Cannot instantiate null as an object");

        if (stream.Length <= 0)
        {
            return (T)buildObject;
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

        return (T)buildObject;
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

    private static object TypeParser(scoped ReadOnlySpan<byte> propertyValue, FileType fileType,
        KeyValueConfiguration config) => fileType switch
    {
        FileType.String => ByteParser.ParseString(propertyValue),
        FileType.Boolean => ByteParser.ParseBool(propertyValue),
        FileType.DateTime => ByteParser.ParseDateTime(propertyValue, config.DateTimeFormat),
        FileType.DateTimeOffset => ByteParser.ParseDateTimeOffset(propertyValue, config.DateTimeFormat),
        FileType.TimeSpan => ByteParser.ParseTimeSpan(propertyValue),
        FileType.Guid => ByteParser.ParseGuid(propertyValue),
        FileType.Int8 => ByteParser.ParseSbyte(propertyValue),
        FileType.UInt8 => ByteParser.ParseByte(propertyValue),
        FileType.Int16 => ByteParser.ParseShort(propertyValue),
        FileType.UInt16 => ByteParser.ParseUshort(propertyValue),
        FileType.Int32 => ByteParser.ParseInt(propertyValue),
        FileType.UInt32 => ByteParser.ParseUint(propertyValue),
        FileType.Int64 => ByteParser.ParseLong(propertyValue),
        FileType.UInt64 => ByteParser.ParseUlong(propertyValue),
        FileType.Float32 => ByteParser.ParseFloat(propertyValue),
        FileType.Float64 => ByteParser.ParseDouble(propertyValue),
        FileType.Float128 => ByteParser.ParseDecimal(propertyValue),
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(fileType), fileType,
            "File Type missing implementation for file parser")
    };
}
