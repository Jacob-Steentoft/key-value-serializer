using System.Buffers;
using System.Buffers.Text;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Helpers;

// ReSharper disable SuggestBaseTypeForParameter

namespace GameSettingSerializer.Serialization;

internal static class ValueWriter
{
	private const int StringChars = 2;
	public static int StringSize(string stringValue) => Encoding.UTF8.GetMaxByteCount(stringValue.Length) + StringChars;

	public static void WriteString(Span<byte> buffer, object value, out int bytesWritten, byte stringSeparator)
	{
		var stringValue = (string)value;

		buffer[0] = stringSeparator;
		var stringByteCount = Encoding.UTF8.GetBytes(stringValue, buffer.Slice(1));
		buffer[stringByteCount + 1] = stringSeparator;

		bytesWritten = stringByteCount + StringChars;
	}

	public const int DateTimeSize = 40;

	public static void WriteDateTime(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out DateTime unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int DateTimeOffsetSize = 40;

	public static void WriteDateTimeOffset(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out DateTimeOffset unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int TimeSpanSize = 36;

	public static void WriteTimeSpan(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out TimeSpan unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	// Found no constant to reference for standard GUID string length
	public const int GuidSize = 36;

	public static void WriteGuid(Span<byte> buffer, object value, out int bytesWritten, StandardFormat format = default)
	{
		if (!value.TryUnbox(out Guid unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int BoolSize = sizeof(bool);

	public static void WriteBool(Span<byte> buffer, object value, out int bytesWritten, StandardFormat format = default)
	{
		if (!value.TryUnbox(out bool unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Int8Size = sizeof(sbyte);

	public static void WriteInt8(Span<byte> buffer, object value, out int bytesWritten, StandardFormat format = default)
	{
		if (!value.TryUnbox(out sbyte unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int UInt8Size = sizeof(byte);

	public static void WriteUInt8(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out byte unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Int16Size = sizeof(short);

	public static void WriteInt16(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out short unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int UInt16Size = sizeof(ushort);

	public static void WriteUInt16(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out ushort unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Int32Size = sizeof(int);

	public static void WriteInt32(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out int unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int UInt32Size = sizeof(uint);

	public static void WriteUInt32(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out uint unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Int64Size = sizeof(long);

	public static void WriteInt64(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out long unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int UInt64Size = sizeof(ulong);

	public static void WriteUInt64(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out ulong unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Float32Size = sizeof(float);

	public static void WriteFloat32(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out float unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Float64Size = sizeof(double);

	public static void WriteFloat64(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out double unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}

	public const int Float128Size = sizeof(decimal);

	public static void WriteFloat128(Span<byte> buffer, object value, out int bytesWritten,
		StandardFormat format = default)
	{
		if (!value.TryUnbox(out decimal unboxedValue))
		{
			ThrowHelper.ThrowFormatException();
		}

		if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out bytesWritten, format))
		{
			ThrowHelper.ThrowFormatException();
		}
	}
}