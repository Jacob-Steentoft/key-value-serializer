using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Cache;

internal static class FileTypeExt
{
    public static FileType GetFileType(this Type? type) => type switch
    {
        _ when type == typeof(string) => FileType.String,
        _ when type == typeof(DateTime) => FileType.DateTime,
        _ when type == typeof(DateTimeOffset) => FileType.DateTimeOffset,
        _ when type == typeof(TimeSpan) => FileType.TimeSpan,
        _ when type == typeof(Guid) => FileType.Guid,
        _ when type == typeof(bool) => FileType.Boolean,
        _ when type == typeof(sbyte) => FileType.Int8,
        _ when type == typeof(byte) => FileType.UInt8,
        _ when type == typeof(short) => FileType.Int16,
        _ when type == typeof(ushort) => FileType.UInt16,
        _ when type == typeof(int) => FileType.Int32,
        _ when type == typeof(uint) => FileType.UInt32,
        _ when type == typeof(long) => FileType.Int64,
        _ when type == typeof(ulong) => FileType.UInt64,
        _ when type == typeof(float) => FileType.Float32,
        _ when type == typeof(double) => FileType.Float64,
        _ when type == typeof(decimal) => FileType.Float128,
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<FileType>(nameof(type), type,
            "Type has not been implemented in cache")
    };
}
