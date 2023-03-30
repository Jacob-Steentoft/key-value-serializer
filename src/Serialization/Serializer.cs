using System.IO.Pipelines;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Serialization;

internal static class Serializer
{
    public static void Serialize<T>(T serverOptions, Stream stream, KeyValueCache cache, KeyValueConfiguration options)
    {
        var pipeWriter = PipeWriter.Create(stream);

        foreach (var property in cache.Properties)
        {
            var propertyValue = property.GetValue(serverOptions);
            if (propertyValue is null)
            {
                continue;
            }

            pipeWriter.WriteKeyNameAndSeparator(options, property);

            if (!property.IsArray)
            {
                pipeWriter.WritePropertyValueAndAdvance(propertyValue, options, property.FileType);
            }
            else
            {
                pipeWriter.WritePropertyArrayValueAndAdvance(propertyValue, options, property.FileType);
            }

            pipeWriter.WriteAndAdvance(options.EndAndNewLine);
        }

        pipeWriter.Complete();
    }

    private static void WriteKeyNameAndSeparator(this PipeWriter pipeWriter, KeyValueConfiguration options,
        KeyValueProperty property)
    {
        var keyNameLength = property.KeyName.Length;
        var keyNameAndSeparatorLength = keyNameLength + options.KeyValueSeparator.Length;

        var keyNameAndSeparator = pipeWriter.GetSpan(keyNameAndSeparatorLength);

        property.KeyName.CopyTo(keyNameAndSeparator);
        options.KeyValueSeparator.CopyTo(keyNameAndSeparator.Slice(keyNameLength));

        pipeWriter.Advance(keyNameAndSeparatorLength);
    }

    private static void WritePropertyArrayValueAndAdvance(this PipeWriter pipeWriter, object propertyValue,
        KeyValueConfiguration config, FileType fileType)
    {
        var propertyValues = (Array)propertyValue;

        pipeWriter.WriteAndAdvance(config.ArrayStart);

        for (var index = 0; index < propertyValues.Length; index++)
        {
            pipeWriter.WritePropertyValueAndAdvance(propertyValues.GetValue(index)!, config, fileType);

            if (index != propertyValues.Length - 1)
            {
                pipeWriter.WriteAndAdvance(config.ArraySeparatorAndSpace);
            }
        }

        pipeWriter.WriteAndAdvance(config.ArrayEnd);
    }

    private static void WriteAndAdvance(this PipeWriter pipeWriter, ReadOnlySpan<byte> value)
    {
        var length = value.Length;
        var span = pipeWriter.GetSpan(length);

        value.CopyTo(span);

        pipeWriter.Advance(length);
    }

    private static void WriteAndAdvance(this PipeWriter pipeWriter, byte value)
    {
        var span = pipeWriter.GetSpan(1);

        span[0] = value;

        pipeWriter.Advance(1);
    }
}
