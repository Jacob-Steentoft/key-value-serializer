using System.Buffers;
using System.Buffers.Text;
using System.IO.Pipelines;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Serialization;

internal static class ValueFormatter
{
    public static void WritePropertyValueAndAdvance(this PipeWriter pipeWriter, object value,
        KeyValueConfiguration config, FileType fileType)
    {
        switch (fileType)
        {
            case FileType.String:
            {
                var stringValue = (string)value;
                const int stringCharCount = 2;

                var maxStringByteCount = Encoding.UTF8.GetMaxByteCount(stringValue.Length) + stringCharCount;

                var buffer = pipeWriter.GetSpan(maxStringByteCount);

                buffer[0] = config.StringSeparator;
                var stringByteCount = Encoding.UTF8.GetBytes(stringValue, buffer.Slice(1));
                buffer[stringByteCount + 1] = config.StringSeparator;

                pipeWriter.Advance(stringByteCount + stringCharCount);
                return;
            }
            case FileType.Boolean:
            {
                const int maxByteSize = sizeof(bool);
                var buffer = pipeWriter.GetSpan(maxByteSize);
                var unboxedValue = (bool)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.DateTime:
            {
                const int maxByteSize = 40;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                var unboxedValue = (DateTime)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten,
                        new StandardFormat(config.DateTimeFormat)))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.DateTimeOffset:
            {
                const int maxByteSize = 40;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                var unboxedValue = (DateTimeOffset)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten,
                        new StandardFormat(config.DateTimeFormat)))
                {
                    ThrowHelper.ThrowFormatException();
                }

                pipeWriter.Advance(bytesWritten);
                return;
            }
            case FileType.TimeSpan:
            {
                const int maxByteSize = 40;
                var buffer = pipeWriter.GetSpan(maxByteSize);
                var unboxedValue = (TimeSpan)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (Guid)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (sbyte)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (byte)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (short)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (ushort)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (int)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (uint)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (long)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (ulong)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (float)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
                var unboxedValue = (double)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten, new StandardFormat('G')))
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
                var unboxedValue = (decimal)value;

                if (!Utf8Formatter.TryFormat(unboxedValue, buffer, out var bytesWritten))
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
