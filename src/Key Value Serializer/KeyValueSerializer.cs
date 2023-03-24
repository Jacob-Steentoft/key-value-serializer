using Key_Value_Serializer.Cache;
using Key_Value_Serializer.Deserialization;
using Key_Value_Serializer.Models;
using Key_Value_Serializer.Serialization;

namespace Key_Value_Serializer;

public static class KeyValueSerializer
{
    private static readonly Dictionary<Type, KeyValueCache> KeyValueCaches = new();
    private static readonly KeyValueConfiguration SerializerOptions = new();

    public static void Serialize<T>(T inputObject, Stream stream, KeyValueConfiguration? config = null) where T : new()
    {
        var cache = GetKeyValueCache<T>();

        var serializerConfiguration = config ?? SerializerOptions;

        Serializer.Serialize(stream, inputObject, cache, serializerConfiguration);
    }

    public static async ValueTask<T> DeserializeAsync<T>(Stream stream, KeyValueConfiguration? config = null,
        CancellationToken cancellationToken = default) where T : new()
    {
        var cache = GetKeyValueCache<T>();

        var serializerConfiguration = config ?? SerializerOptions;

        return await SettingDeserializer.DeserializeStreamAsync<T>(stream, cache, serializerConfiguration,
            cancellationToken);
    }

    private static KeyValueCache GetKeyValueCache<T>() where T : new()
    {
        var type = typeof(T);
        if (KeyValueCaches.TryGetValue(type, out var cache))
        {
            return cache;
        }
        
        cache = new KeyValueCache(type);
        KeyValueCaches.Add(type, cache);

        return cache;
    }
}
