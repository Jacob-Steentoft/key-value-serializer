using System.Reflection;
using System.Text;
using CommunityToolkit.Diagnostics;
using Fasterflect;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Cache;

internal sealed class KeyValueProperty
{
    public KeyValueProperty(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            propertyType = Nullable.GetUnderlyingType(propertyType);
        }

        if (propertyType is null)
        {
            ThrowHelper.ThrowInvalidOperationException("Property type cannot be null");
        }

        var attribute = property.GetCustomAttribute<KeyFileName>();

        var propertyName = attribute is null ? property.Name : attribute.Name;
        var propertyBytes = Encoding.UTF8.GetBytes(propertyName);

        var baseType = propertyType.IsArray
            ? propertyType.GetElementType().GetFileType()
            : propertyType.GetFileType();

        KeyName = propertyBytes;
        FileType = baseType;
        IsArray = propertyType.IsArray;
        SetValue = property.DelegateForSetPropertyValue();
        GetValue = property.DelegateForGetPropertyValue();
    }

    public byte[] KeyName { get; }
    public bool IsArray { get; }
    public FileType FileType { get; }
    public MemberSetter SetValue { get; }
    public MemberGetter GetValue { get; }
}
