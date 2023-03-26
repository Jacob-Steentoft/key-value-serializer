using System.Buffers.Text;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Serialization;

internal static class Serializer
{
    public static void Serialize<T>(T serverOptions, Stream stream, KeyValueCache cache, KeyValueConfiguration options)
    {
        var pipeWriter = PipeWriter.Create(stream);

        // Move to configuration?
        Span<byte> keyValueSeparator = stackalloc byte[3];
        keyValueSeparator[0] = options.Space;
        keyValueSeparator[1] = options.ValueStart;
        keyValueSeparator[2] = options.Space;
        
        Span<byte> newKeyValuePair = stackalloc byte[1 + options.NewLine.Length];
        newKeyValuePair[0] = options.ValueEnd;
        options.NewLine.CopyTo(newKeyValuePair.Slice(1));

        foreach (var property in cache.Properties)
        {
            // Get the value from the object to be serialized, if no value is assigned the key and value is skipped
            var propertyValue = property.GetValue(serverOptions);
            if (propertyValue is null)
            {
                continue;
            }

            // Write key name and separator to the stream
            var keyNameLength = property.KeyName.Length;
            var keyNameAndSeparatorLength = keyNameLength + keyValueSeparator.Length;

            var keyNameAndSeparator = pipeWriter.GetSpan(keyNameAndSeparatorLength);

            property.KeyName.CopyTo(keyNameAndSeparator);
            keyValueSeparator.CopyTo(keyNameAndSeparator.Slice(keyNameLength));

            pipeWriter.Advance(keyNameAndSeparatorLength);

            // Write value to the stream
            if (!property.IsArray)
            {
                pipeWriter.WritePropertyValueAndAdvance(propertyValue, options, property.FileType);
            }
            else
            {
                pipeWriter.WritePropertyArrayValueAndAdvance(propertyValue, options, property.FileType);
            }

            // Write the value end and start a new line
            pipeWriter.WriteAndAdvance(newKeyValuePair);
        }

        pipeWriter.Complete();
    }

    private static void WritePropertyArrayValueAndAdvance(this PipeWriter pipeWriter, object propertyValue,
        KeyValueConfiguration options, FileType fileType)
    {
        var propertyValues = (Array)propertyValue;
        Span<byte> span = stackalloc byte[2];
        span[0] = options.ArraySeparator;
        span[1] = options.Space;
        
        pipeWriter.WriteAndAdvance(options.ArrayStart);

        for (var index = 0; index < propertyValues.Length; index++)
        {
            pipeWriter.WritePropertyValueAndAdvance(propertyValues.GetValue(index)!, options, fileType);
            
            if (index != propertyValues.Length - 1)
            {
                pipeWriter.WriteAndAdvance(span);
                
            }
        }

        pipeWriter.WriteAndAdvance(options.ArrayEnd);
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
