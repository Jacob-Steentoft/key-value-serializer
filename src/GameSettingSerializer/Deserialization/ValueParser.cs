using System.Buffers.Text;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;

namespace GameSettingSerializer.Deserialization;

internal static class ValueParser
{
    public static string ParseString(scoped ReadOnlySpan<byte> buffer)
    {
        return StringPool.Shared.GetOrAdd(buffer, Encoding.UTF8);
    }

    public static DateTime ParseDateTime(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out DateTime value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'DateTime' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static DateTimeOffset ParseDateTimeOffset(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out DateTimeOffset value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'DateTimeOffset' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static TimeSpan ParseTimeSpan(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out TimeSpan value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'TimeSpan' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static bool ParseBool(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out bool value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'bool' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static Guid ParseGuid(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out Guid value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'Guid' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static sbyte ParseInt8(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out sbyte value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'sbyte' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static byte ParseUInt8(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out byte value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'byte' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static short ParseInt16(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out short value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'short' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static ushort ParseUInt16(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out ushort value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'ushort' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static int ParseInt32(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out int value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'int' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static uint ParseUInt32(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out uint value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'uint' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static long ParseInt64(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out long value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'long' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static ulong ParseUInt64(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out ulong value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'ulong' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static float ParseFloat32(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out float value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'float' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static double ParseFloat64(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out double value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'double' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }

    public static decimal ParseFloat128(scoped ReadOnlySpan<byte> buffer, char format = default)
    {
        if (!Utf8Parser.TryParse(buffer, out decimal value, out _, format))
        {
            ThrowHelper.ThrowFormatException(
                $"Unable to parse 'decimal' type from the following data: '{Encoding.UTF8.GetString(buffer)}'");
        }

        return value;
    }
}
