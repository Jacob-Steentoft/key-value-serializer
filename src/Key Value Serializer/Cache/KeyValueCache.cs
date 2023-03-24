using System.Reflection;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Helpers;
using Fasterflect;
using Key_Value_Serializer.Models;

namespace Key_Value_Serializer.Cache;

internal sealed class KeyValueCache
{
    public KeyValueCache(IReflect type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        Properties = new KeyValueProperty[properties.Length];
        _lookupTable = new int[properties.Length];

        for (var index = 0; index < properties.Length; index++)
        {
            var property = properties[index];
            var propertyType = property.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = Nullable.GetUnderlyingType(propertyType);
            }

            if (propertyType is null)
            {
                ThrowHelper.ThrowInvalidOperationException("Property type cannot be null");
            }

            var attribute = property.GetCustomAttribute(typeof(KeyFileName)) as KeyFileName;

            var propertyName = attribute is null ? property.Name : attribute.Name;
            var propertyBytes = Encoding.UTF8.GetBytes(propertyName);

            var nonArrayType = propertyType.IsArray
                ? GetFileType(propertyType.GetElementType())
                : GetFileType(propertyType);

            var settingProperty = new KeyValueProperty
            {
                KeyName = propertyBytes,
                FileType = nonArrayType,
                IsArray = propertyType.IsArray,
                SetValue = property.DelegateForSetPropertyValue(),
                GetValue = property.DelegateForGetPropertyValue()
            };

            _lookupTable[index] = HashCode<byte>.Combine(propertyBytes);
            Properties[index] = settingProperty;
        }
    }

    public readonly KeyValueProperty[] Properties;
    private readonly int[] _lookupTable;

    public KeyValueProperty GetKeyValueProperty(scoped ReadOnlySpan<byte> settingName)
    {
        var hash = HashCode<byte>.Combine(settingName);
        var index = _lookupTable.AsSpan().IndexOf(hash);
        return Properties[index];
    }

    private static FileType GetFileType(Type? type) => type switch
    {
        _ when type == typeof(string) => FileType.String,
        _ when type == typeof(DateTime) => FileType.DateTime,
        _ when type == typeof(DateTimeOffset) => FileType.DateTimeOffset,
        _ when type == typeof(TimeSpan) => FileType.TimeSpan,
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
        _ => ThrowHelper.ThrowArgumentOutOfRangeException<FileType>(nameof(type), type, "Type has not been implemented")
    };
}
