using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using GameSettingSerializer.Cache;
using GameSettingSerializer.Data;

namespace GameSettingSerializer.Deserialization;

internal sealed class AsyncDeserializer<T> where T : new()
{
	private readonly KeyValueConfiguration _configuration;
	private readonly KeyValueCache _cache;

	public AsyncDeserializer(KeyValueConfiguration configuration, KeyValueCache cache)
	{
		_configuration = configuration;
		_cache = cache;
	}

	public async ValueTask<T> DeserializeAsync(Stream stream, CancellationToken cancellationToken)
	{
		Guard.CanRead(stream);
		Guard.CanSeek(stream);
		object buildObject = new T();
		Guard.IsNotNull(buildObject, "Cannot instantiate null as an object");
		
		KeyValueProperty? property = null;
		const int capacity = 512;
		var poolBufferWriter = new ArrayPoolBufferWriter<byte>(capacity);
		
		while (true)
		{
			var memory = poolBufferWriter.GetMemory(capacity);
			var readAsync = await stream.ReadAsync(memory, cancellationToken);
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
	private SequencePosition ProcessBuffer(ReadOnlySequence<byte> sequence, ref KeyValueProperty? property,
		ref object buildObject)
	{
		var reader = new SequenceReader<byte>(sequence);

		long consumedBytes;
		do
		{
			consumedBytes = reader.Consumed;

			AdvancePastFiller(reader);

			// Skip Comments
			if (!TryAdvancePastComment(reader, out var moreComments))
			{
				break;
			}

			if (moreComments)
			{
				continue;
			}

			// Get Property Name
			if (property is null)
			{
				if (!TryGetProperty(reader, out property))
				{
					break;
				}
			}

			object value;

			// Get Property Value
			if (property.IsArray)
			{
				if (TryGetArray(reader, out value))
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

	private bool TryGetProperty(scoped SequenceReader<byte> reader,
		[MaybeNullWhen(false)] out KeyValueProperty property)
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

	private bool TryGetValue(scoped SequenceReader<byte> reader, SupportedFileTypes fileType,
		[MaybeNullWhen(false)] out object value)
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

	private object TypeParser(scoped ReadOnlySpan<byte> propertyValue, SupportedFileTypes fileType) => fileType switch
	{
		SupportedFileTypes.String => ValueParser.ParseString(propertyValue),
		SupportedFileTypes.Boolean => ValueParser.ParseBool(propertyValue),
		SupportedFileTypes.DateTime => ValueParser.ParseDateTime(propertyValue, _configuration.DateTimeFormat),
		SupportedFileTypes.DateTimeOffset => ValueParser.ParseDateTimeOffset(propertyValue,
			_configuration.DateTimeFormat),
		SupportedFileTypes.TimeSpan => ValueParser.ParseTimeSpan(propertyValue),
		SupportedFileTypes.Guid => ValueParser.ParseGuid(propertyValue),
		SupportedFileTypes.Int8 => ValueParser.ParseInt8(propertyValue),
		SupportedFileTypes.UInt8 => ValueParser.ParseUInt8(propertyValue),
		SupportedFileTypes.Int16 => ValueParser.ParseInt16(propertyValue),
		SupportedFileTypes.UInt16 => ValueParser.ParseUInt16(propertyValue),
		SupportedFileTypes.Int32 => ValueParser.ParseInt32(propertyValue),
		SupportedFileTypes.UInt32 => ValueParser.ParseUInt32(propertyValue),
		SupportedFileTypes.Int64 => ValueParser.ParseInt64(propertyValue),
		SupportedFileTypes.UInt64 => ValueParser.ParseUInt64(propertyValue),
		SupportedFileTypes.Float32 => ValueParser.ParseFloat32(propertyValue),
		SupportedFileTypes.Float64 => ValueParser.ParseFloat64(propertyValue),
		SupportedFileTypes.Float128 => ValueParser.ParseFloat128(propertyValue),
		_ => ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(fileType), fileType,
			"File Type missing implementation for file parser")
	};
}