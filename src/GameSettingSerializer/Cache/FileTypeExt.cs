using CommunityToolkit.Diagnostics;
using GameSettingSerializer.Data;

namespace GameSettingSerializer.Cache;

internal static class FileTypeExt
{
    public static SupportedFileTypes GetFileType(this Type? type) => type switch
    {
        _ when type == typeof(string) => SupportedFileTypes.String,
        _ when type == typeof(DateTime) => SupportedFileTypes.DateTime,
        _ when type == typeof(DateTimeOffset) => SupportedFileTypes.DateTimeOffset,
        _ when type == typeof(TimeSpan) => SupportedFileTypes.TimeSpan,
        _ when type == typeof(Guid) => SupportedFileTypes.Guid,
        _ when type == typeof(bool) => SupportedFileTypes.Boolean,
        _ when type == typeof(sbyte) => SupportedFileTypes.Int8,
        _ when type == typeof(byte) => SupportedFileTypes.UInt8,
        _ when type == typeof(short) => SupportedFileTypes.Int16,
        _ when type == typeof(ushort) => SupportedFileTypes.UInt16,
        _ when type == typeof(int) => SupportedFileTypes.Int32,
        _ when type == typeof(uint) => SupportedFileTypes.UInt32,
        _ when type == typeof(long) => SupportedFileTypes.Int64,
        _ when type == typeof(ulong) => SupportedFileTypes.UInt64,
        _ when type == typeof(float) => SupportedFileTypes.Float32,
        _ when type == typeof(double) => SupportedFileTypes.Float64,
        _ when type == typeof(decimal) => SupportedFileTypes.Float128,
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<SupportedFileTypes>(nameof(type), type,
            "Type has not been implemented in cache")
    };
}
