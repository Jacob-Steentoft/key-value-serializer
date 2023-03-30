using System.Reflection;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Helpers;

namespace KeyValueSerializer.Cache;

internal sealed class KeyValueCache
{
    public KeyValueCache(IReflect type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        Properties = new KeyValueProperty[properties.Length];
        _lookupTable = new int[properties.Length];

        for (var index = 0; index < properties.Length; index++)
        {
            var settingProperty = new KeyValueProperty(properties[index]);

            var hash = CalculateHash(settingProperty.KeyName);

            if (_lookupTable.Contains(hash))
            {
                ThrowHelper.ThrowInvalidOperationException(
                    $"{settingProperty.KeyName} already exists in the collection");
            }

            _lookupTable[index] = hash;
            Properties[index] = settingProperty;
        }
    }

    public readonly KeyValueProperty[] Properties;
    private readonly int[] _lookupTable;

    public bool TryGetKeyValueProperty(scoped ReadOnlySpan<byte> settingName, out KeyValueProperty? property)
    {
        var hash = CalculateHash(settingName);
        var index = _lookupTable.AsSpan().IndexOf(hash);
        if (index < 0)
        {
            property = null;
            return false;
        }

        property = Properties[index];
        return true;
    }

    private static int CalculateHash(scoped ReadOnlySpan<byte> settingName) => HashCode<byte>.Combine(settingName);
}
