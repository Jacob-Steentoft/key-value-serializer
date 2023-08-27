using CommunityToolkit.HighPerformance.Buffers;
using GameSettingSerializer.Cache;

namespace GameSettingSerializer.Serialization;

internal static class Serializer
{
    public static async ValueTask SerializeAsync<T>(T serverOptions, Stream stream, KeyValueCache cache, KeyValueConfiguration config)
    {
        using var bufferWriter = new ArrayPoolBufferWriter<byte>();
        foreach (var property in cache.Properties)
        {
            var propertyValue = property.GetValue(serverOptions);
            if (propertyValue is null)
            {
                continue;
            }

            bufferWriter.WriteKeyNameAndSeparator(config, property);

            bufferWriter.WritePropertyAndAdvance(propertyValue, config, property);

            bufferWriter.WriteEndAndAdvance(config);
            
            await stream.WriteAsync(bufferWriter.WrittenMemory);
            bufferWriter.Clear();
        }
    }
}
