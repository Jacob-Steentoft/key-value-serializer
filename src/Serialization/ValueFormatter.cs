using System.Buffers;
using System.Buffers.Text;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Serialization;

internal static class ValueFormatter
{
    public static void WritePropertyValueAndAdvance(this PipeWriter pipeWriter, object propertyValue,
        KeyValueConfiguration config, FileType fileType)
    {
        switch (fileType)
        {
            case FileType.String:
            {
                var stringValue = (string)propertyValue;

                var stringByteCount = Encoding.UTF8.GetByteCount(stringValue);
                var stringContentLength = stringByteCount + 2;

                var buffer = pipeWriter.GetSpan(stringContentLength);

                Encoding.UTF8.GetBytes(stringValue, buffer.Slice(1, stringByteCount));
                buffer[0] = config.StringSeparator;
                buffer[stringContentLength - 1] = config.StringSeparator;

                pipeWriter.Advance(stringContentLength);
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
                // TODO: Look into proper size
                const int maxByteSize = 100;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((DateTime)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.DateTimeOffset:
            {
                // TODO: Look into proper size
                const int maxByteSize = 100;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((DateTimeOffset)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.TimeSpan:
            {
                // TODO: Look into proper size
                const int maxByteSize = 100;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((TimeSpan)propertyValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.Guid:
            {
                // Found no constant to reference for standard GUID string length
                const int maxByteSize = 36;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                if (!Utf8Formatter.TryFormat((Guid)propertyValue, buffer, out var bytesWritten))
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
                if (!Utf8Formatter.TryFormat((double)propertyValue, buffer, out var bytesWritten,new StandardFormat('G')))
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
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(fileType), fileType,
                    "File Type missing implementation");
                return;
            }
        }
    }
}
