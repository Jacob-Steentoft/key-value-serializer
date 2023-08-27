using GameSettingSerializer.Cache;
using GameSettingSerializer.Serialization;

namespace GameSettingSerializer;

public static class KeyValueSerializer
{
    private static readonly Dictionary<Type, KeyValueCache> KeyValueCaches = new();
    private static readonly KeyValueConfiguration SerializerOptions = new();

    public static async ValueTask SerializeAsync<T>(T inputObject, Stream stream, KeyValueConfiguration? config = null) where T : new()
    {
        var cache = GetKeyValueCache<T>();

        var serializerConfiguration = config ?? SerializerOptions;

        await Serializer.SerializeAsync(inputObject, stream, cache, serializerConfiguration);
    }

    public static async ValueTask<T> DeserializeAsync<T>(Stream stream, KeyValueConfiguration? config = null,
        CancellationToken cancellationToken = default) where T : new()
    {
        var cache = GetKeyValueCache<T>();

        var serializerConfiguration = config ?? SerializerOptions;

        return await Deserializer.DeserializeStreamAsync<T>(stream, cache, serializerConfiguration,
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
