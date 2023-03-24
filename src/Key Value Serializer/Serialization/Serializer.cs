using System.Buffers.Text;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using Key_Value_Serializer.Cache;
using Key_Value_Serializer.Models;

namespace Key_Value_Serializer.Serialization;

internal static class Serializer
{
    public static void Serialize<T>(Stream stream, T serverOptions, KeyValueCache cache,
        KeyValueConfiguration options)
    {
        var pipeWriter = PipeWriter.Create(stream);

        Span<byte> keyValueSeparator = stackalloc byte[3];
        keyValueSeparator[0] = options.Space;
        keyValueSeparator[1] = options.ValueStart;
        keyValueSeparator[2] = options.Space;
        
        Span<byte> newKeyValuePair = stackalloc byte[1 + options.NewLine.Length];
        keyValueSeparator[0] = options.ValueEnd;
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
                pipeWriter.WritePropertyValueAndAdvance(propertyValue, property, options);
            }
            else
            {
                pipeWriter.WritePropertyArrayValueAndAdvance(propertyValue, property, options);
            }

            // Write the value end and start a new line
            pipeWriter.WriteAndAdvance(newKeyValuePair);
        }

        pipeWriter.Complete();
    }

    private static void WritePropertyArrayValueAndAdvance(this PipeWriter pipeWriter, object propertyValue,
        KeyValueProperty property, KeyValueConfiguration options)
    {
        var propertyValues = (object[])propertyValue;

        pipeWriter.WriteAndAdvance(options.ArrayStart);

        for (var index = 0; index < propertyValues.Length; index++)
        {
            pipeWriter.WritePropertyValueAndAdvance(propertyValues[index], property, options);

            if (index == propertyValues.Length - 1)
            {
                pipeWriter.WriteAndAdvance(options.ArrayEnd);
                return;
            }

            pipeWriter.WriteAndAdvance(options.ArraySeparator);
        }
    }

    private static void WritePropertyValueAndAdvance(this PipeWriter pipeWriter, object propertyValue,
        KeyValueProperty property, KeyValueConfiguration options)
    {
        switch (property.FileType)
        {
            case FileType.String:
            {
                var stringValue = (string)propertyValue;
                var stringByteCount = Encoding.UTF8.GetByteCount(stringValue);
                var buffer = pipeWriter.GetSpan(stringByteCount + 2);
                Encoding.UTF8.GetBytes(stringValue, buffer.Slice(1, stringByteCount));
                buffer[0] = options.StringSeparator;
                buffer[buffer.Length - 1] = options.StringSeparator;

                pipeWriter.Advance(stringByteCount);
                return;
            }
            case FileType.Boolean:
            {
                const int maxByteSize = sizeof(bool);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((bool)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.DateTime:
            {
                ThrowHelper.ThrowFormatException();
                return;
            }
            case FileType.DateTimeOffset:
            {
                ThrowHelper.ThrowFormatException();
                return;
            }
            case FileType.TimeSpan:
            {
                ThrowHelper.ThrowFormatException();
                return;
            }
            case FileType.Guid:
            {
                // Found no constant to reference for standard GUID string length
                const int maxByteSize = 36;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((DateTime)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Int8:
            {
                const int maxByteSize = sizeof(sbyte);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((sbyte)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.UInt8:
            {
                const int maxByteSize = sizeof(byte);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((byte)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Int16:
            {
                const int maxByteSize = sizeof(short);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((short)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.UInt16:
            {
                const int maxByteSize = sizeof(ushort);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((ushort)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Int32:
            {
                const int maxByteSize = sizeof(int);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((int)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.UInt32:
            {
                const int maxByteSize = sizeof(uint);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((uint)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Int64:
            {
                const int maxByteSize = sizeof(long);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((long)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.UInt64:
            {
                const int maxByteSize = sizeof(ulong);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((ulong)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Float32:
            {
                const int maxByteSize = sizeof(float);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((float)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Float64:
            {
                const int maxByteSize = sizeof(double);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((double)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Float128:
            {
                const int maxByteSize = sizeof(decimal);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((decimal)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            default:
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(property.FileType));
                return;
            }
        }
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
