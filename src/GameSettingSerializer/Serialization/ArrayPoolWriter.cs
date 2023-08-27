using System.Buffers;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using GameSettingSerializer.Cache;
using GameSettingSerializer.Data;

// ReSharper disable SuggestBaseTypeForParameter

namespace GameSettingSerializer.Serialization;

internal static class ArrayPoolWriter
{
	public static void WriteKeyNameAndSeparator(this ArrayPoolBufferWriter<byte> writer, KeyValueConfiguration options,
		KeyValueProperty property)
	{
		var keyNameLength = property.KeyName.Length;
		var keyNameAndSeparatorLength = keyNameLength + options.KeyValueSeparator.Length;

		var keyNameAndSeparator = writer.GetSpan(keyNameAndSeparatorLength);

		property.KeyName.CopyTo(keyNameAndSeparator);
		options.KeyValueSeparator.CopyTo(keyNameAndSeparator.Slice(keyNameLength));

		writer.Advance(keyNameAndSeparatorLength);
	}

	public static void WritePropertyAndAdvance(this ArrayPoolBufferWriter<byte> writer, object propertyValue,
		KeyValueConfiguration config, KeyValueProperty property)
	{
		if (!property.IsArray)
		{
			WritePropertyValueAndAdvance(ref writer, propertyValue, config, property.FileType);
			return;
		}

		WritePropertyArrayValueAndAdvance(ref writer, propertyValue, config, property.FileType);
	}

	public static void WriteEndAndAdvance(this ArrayPoolBufferWriter<byte> writer, KeyValueConfiguration options)
	{
		WriteAndAdvance(ref writer, options.EndAndNewLine);
	}

	private static void WritePropertyArrayValueAndAdvance(ref ArrayPoolBufferWriter<byte> writer, object propertyValue,
		KeyValueConfiguration config, SupportedFileTypes fileType)
	{
		var propertyValues = (Array)propertyValue;

		WriteAndAdvance(ref writer, config.ArrayStart);

		for (var index = 0; index < propertyValues.Length; index++)
		{
			WritePropertyValueAndAdvance(ref writer, propertyValues.GetValue(index)!, config, fileType);

			if (index != propertyValues.Length - 1)
			{
				WriteAndAdvance(ref writer, config.ArraySeparatorAndSpace);
			}
		}

		WriteAndAdvance(ref writer, config.ArrayEnd);
	}

	// ReSharper disable once CognitiveComplexity
	private static void WritePropertyValueAndAdvance(ref ArrayPoolBufferWriter<byte> writer, object propertyValue,
		KeyValueConfiguration config, SupportedFileTypes fileType)
	{
		int bytesWritten;
		switch (fileType)
		{
			case SupportedFileTypes.String:
			{
				var buffer = writer.GetSpan(ValueWriter.StringSize((string)propertyValue));
				ValueWriter.WriteString(buffer, propertyValue, out bytesWritten, config.StringSeparator);
				break;
			}
			case SupportedFileTypes.DateTime:
			{
				var buffer = writer.GetSpan(ValueWriter.DateTimeSize);
				ValueWriter.WriteDateTime(buffer, propertyValue, out bytesWritten, new StandardFormat(config.DateTimeFormat));
				break;
			}
			case SupportedFileTypes.DateTimeOffset:
			{
				var buffer = writer.GetSpan(ValueWriter.DateTimeOffsetSize);
				ValueWriter.WriteDateTimeOffset(buffer, propertyValue, out bytesWritten, new StandardFormat(config.DateTimeFormat));
				break;
			}
			case SupportedFileTypes.TimeSpan:
			{
				var buffer = writer.GetSpan(ValueWriter.TimeSpanSize);
				ValueWriter.WriteTimeSpan(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Boolean:
			{
				var buffer = writer.GetSpan(ValueWriter.BoolSize);
				ValueWriter.WriteBool(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Guid:
			{
				var buffer = writer.GetSpan(ValueWriter.GuidSize);
				ValueWriter.WriteGuid(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Int8:
			{
				var buffer = writer.GetSpan(ValueWriter.Int8Size);
				ValueWriter.WriteInt8(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.UInt8:
			{
				var buffer = writer.GetSpan(ValueWriter.UInt8Size);
				ValueWriter.WriteUInt8(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Int16:
			{
				var buffer = writer.GetSpan(ValueWriter.Int16Size);
				ValueWriter.WriteInt16(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.UInt16:
			{
				var buffer = writer.GetSpan(ValueWriter.UInt16Size);
				ValueWriter.WriteUInt16(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Int32:
			{
				var buffer = writer.GetSpan(ValueWriter.Int32Size);
				ValueWriter.WriteInt32(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.UInt32:
			{
				var buffer = writer.GetSpan(ValueWriter.UInt32Size);
				ValueWriter.WriteUInt32(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Int64:
			{
				var buffer = writer.GetSpan(ValueWriter.Int64Size);
				ValueWriter.WriteInt64(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.UInt64:
			{
				var buffer = writer.GetSpan(ValueWriter.UInt64Size);
				ValueWriter.WriteUInt64(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Float32:
			{
				var buffer = writer.GetSpan(ValueWriter.Float32Size);
				ValueWriter.WriteFloat32(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Float64:
			{
				var buffer = writer.GetSpan(ValueWriter.Float64Size);
				ValueWriter.WriteFloat64(buffer, propertyValue, out bytesWritten);
				break;
			}
			case SupportedFileTypes.Float128:
			{
				var buffer = writer.GetSpan(ValueWriter.Float128Size);
				ValueWriter.WriteFloat128(buffer, propertyValue, out bytesWritten);
				break;
			}
			default:
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(nameof(fileType), fileType,
					"File Type missing implementation");
				return;
			}
		}
		writer.Advance(bytesWritten);
	}

	private static void WriteAndAdvance(ref ArrayPoolBufferWriter<byte> writer, ReadOnlySpan<byte> value)
	{
		var length = value.Length;
		var span = writer.GetSpan(length);

		value.CopyTo(span);

		writer.Advance(length);
	}

	private static void WriteAndAdvance(ref ArrayPoolBufferWriter<byte> writer, byte value)
	{
		var span = writer.GetSpan(1);

		span[0] = value;

		writer.Advance(1);
	}
}