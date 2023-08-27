using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Deserialization;

internal sealed class AsyncDeserializer<T> : IAsyncDisposable where T : new()
{
    private readonly PipeReader _pipeReader;
    private readonly KeyValueConfiguration _configuration;
    private readonly KeyValueCache _cache;

    public AsyncDeserializer(Stream stream, KeyValueConfiguration configuration, KeyValueCache cache)
    {
        Guard.CanRead(stream);
        Guard.CanSeek(stream);
        _pipeReader = PipeReader.Create(stream);

        _configuration = configuration;
        _cache = cache;
    }

    public async ValueTask<T> DeserializeAsync(CancellationToken cancellationToken)
    {
        KeyValueProperty? property = null;
        object buildObject = new T();
        
        Guard.IsNotNull(buildObject, "Cannot instantiate null as an object");

        while (true)
        {
            var result = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            var buffer = result.Buffer;

            var sequencePosition = ProcessBuffer(buffer, ref property, ref buildObject);

            if (result.IsCompleted)
            {
                break;
            }

            _pipeReader.AdvanceTo(sequencePosition, buffer.End);
        }

        return (T)buildObject;
    }

    // Breaks loop to get more data for buffer
    private SequencePosition ProcessBuffer(ReadOnlySequence<byte> sequence, ref KeyValueProperty? property, ref object buildObject)
    {
        var reader = new SequenceReader<byte>(sequence);

        long consumedBytes;
        do
        {
            consumedBytes = reader.Consumed;

            AdvancePastFiller(reader);

            if (!TryAdvancePastComment(reader, out var moreComments))
            {
                break;
            }

            if (moreComments)
            {
                continue;
            }

            if (property is null)
            {
                if (!TryGetProperty(reader, out property))
                {
                    break;
                }
            }

            object value;

            if (property.IsArray)
            {
                if (TryGetArray(reader, out value))
                {
                    break;
                }
            }
            else if (property.FileType is FileType.String)
            {
                if (TryGetString(reader, out value))
                {
                    break;
                }
            }
            else
            {
                if (!TryGetValue(reader, property.FileType, out value))
                {
                    break;
                }
            }

            property.SetValue(buildObject, value);
            property = null;

            consumedBytes = reader.Consumed;
        } while (!reader.End && reader.Consumed > consumedBytes);

        return reader.Position;
    }

    private void AdvancePastFiller(scoped SequenceReader<byte> reader)
    {
        reader.AdvancePastAny(_configuration.SkipFiller);
    }

    private bool TryAdvancePastComment(scoped SequenceReader<byte> reader, out bool canHaveMoreComments)
    {
        if (!reader.IsNext(_configuration.CommentStart))
        {
            canHaveMoreComments = false;
            return true;
        }

        if (!reader.TryAdvanceToAny(_configuration.CommentEnd))
        {
            canHaveMoreComments = true;
            return false;
        }

        canHaveMoreComments = true;
        return true;
    }

    private bool TryGetProperty(scoped SequenceReader<byte> reader, [MaybeNullWhen(false)] out KeyValueProperty property)
    {
        var readerUnreadSpan = reader.UnreadSpan;

        var valueStartIndex = readerUnreadSpan.IndexOf(_configuration.ValueStart);
        if (valueStartIndex is -1)
        {
            property = default;
            return false;
        }

        var propertyName = readerUnreadSpan.Slice(0, valueStartIndex).Trim(_configuration.SkipFiller);
        if (!_cache.TryGetKeyValueProperty(propertyName, out property))
        {
            ThrowHelper.ThrowInvalidOperationException(
                $"The key '{Encoding.UTF8.GetString(propertyName)}' was not found in the type");
        }

        reader.Advance(valueStartIndex + 1);
        return true;
    }

    private bool TryGetValue(scoped SequenceReader<byte> reader, FileType fileType, [MaybeNullWhen(false)] out object value)
    {
        reader.AdvancePastAny(_configuration.WhiteSpaces);

        if (!reader.TryReadTo(out ReadOnlySpan<byte> valueSpan, _configuration.ValueEnd))
        {
            value = default;
            return false;
        }

        value = TypeParser(valueSpan, fileType);
        return true;
    }

    private object TypeParser(scoped ReadOnlySpan<byte> propertyValue, FileType fileType) => fileType switch
    {
        FileType.Boolean => ByteParser.ParseBool(propertyValue),
        FileType.DateTime => ByteParser.ParseDateTime(propertyValue, _configuration.DateTimeFormat),
        FileType.DateTimeOffset => ByteParser.ParseDateTimeOffset(propertyValue, _configuration.DateTimeFormat),
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

    public async ValueTask DisposeAsync()
    {
        await _pipeReader.CompleteAsync();
    }
}
