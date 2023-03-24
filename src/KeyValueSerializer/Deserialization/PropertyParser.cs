using System.Buffers.Text;
using System.Text;
using CommunityToolkit.Diagnostics;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Deserialization;

internal static class PropertyParser
{
    public static void SetProperty(object buildObject, KeyValueProperty property, scoped ReadOnlySpan<byte> propertyValue,
        KeyValueConfiguration options)
    {
        var objectValue = ParseFileProperty(propertyValue, property.FileType);
        property.SetValue(buildObject, objectValue);
    }

    public static void SetArrayProperty(object buildObject, KeyValueProperty property,
        ReadOnlySpan<byte> propertyValue, KeyValueConfiguration options, int arraySize)
    {
        switch (property.FileType)
        {
            case FileType.String:
            {
                SetArray<string>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Boolean:
            {
                SetArray<bool>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.DateTime:
            {
                SetArray<DateTime>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.DateTimeOffset:
            {
                SetArray<DateTimeOffset>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.TimeSpan:
            {
                SetArray<TimeSpan>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Guid:
            {
                SetArray<Guid>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Int8:
            {
                SetArray<sbyte>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.UInt8:
            {
                SetArray<byte>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Int16:
            {
                SetArray<short>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.UInt16:
            {
                SetArray<ushort>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Int32:
            {
                SetArray<int>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.UInt32:
            {
                SetArray<uint>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Int64:
            {
                SetArray<long>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.UInt64:
            {
                SetArray<ulong>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Float32:
            {
                SetArray<float>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Float64:
            {
                SetArray<double>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            case FileType.Float128:
            {
                SetArray<decimal>(buildObject, property, propertyValue, options, arraySize);
                break;
            }
            default:
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
                break;
            }
        }
        
    }

    private static void SetArray<T>(object buildObject, KeyValueProperty property,
        scoped ReadOnlySpan<byte> arrayBytes, KeyValueConfiguration options, int arraySize)
    {
        var objectValues = new T[arraySize];
        
        for (var index = 0; index < objectValues.Length; index++)
        {
            var itemBytes = GetArrayItem(arrayBytes, options, out var nextIndex);
            objectValues[index] = (T)ParseFileProperty(itemBytes, property.FileType);

            arrayBytes = arrayBytes.Slice(nextIndex);
        }

        property.SetValue(buildObject, objectValues);
    }

    private static ReadOnlySpan<byte> GetArrayItem(ReadOnlySpan<byte> buffer, KeyValueConfiguration options, out int nextIndex)
    {
        var startIndex = -1;
        for (var index = 0; index < buffer.Length; index++)
        {
            var bufferByte = buffer[index];

            if (bufferByte == options.StringSeparator)
            {
                startIndex = index;
                var stringSeparatorIndex = index;
                do
                {
                    stringSeparatorIndex = buffer.Slice(stringSeparatorIndex + 1).IndexOf(options.StringSeparator);
                    if (stringSeparatorIndex < 0)
                    {
                        nextIndex = -1;
                        return ReadOnlySpan<byte>.Empty;
                    }
                } while (buffer[stringSeparatorIndex - 1] == options.StringIgnoreCharacter);

                nextIndex = startIndex + 1 + stringSeparatorIndex + 1;
                return buffer.Slice(startIndex + 1, stringSeparatorIndex - startIndex + 1);
            }

            if (bufferByte != options.ArrayStart ||
                bufferByte != options.ArraySeparator ||
                bufferByte != options.ArrayEnd)
            {
                continue;
            }

            if (startIndex == -1)
            {
                startIndex = index;
                continue;
            }

            nextIndex = index - startIndex;
            return buffer.Slice(startIndex + 1, index - startIndex - 1);
        }

        nextIndex = -1;
        return ReadOnlySpan<byte>.Empty;
    }

    private static object ParseFileProperty(scoped ReadOnlySpan<byte> propertyValue, FileType fileType)
    {
        switch (fileType)
        {
            case FileType.String:
            {
                return propertyValue.Length == 0 ? string.Empty : Encoding.UTF8.GetString(propertyValue);
            }
            case FileType.Boolean:
            {
                if (!Utf8Parser.TryParse(propertyValue, out bool value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse boolean type");
                }

                return value;
            }
            case FileType.DateTime:
            {
                if (!Utf8Parser.TryParse(propertyValue, out DateTime value, out _, 'R'))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse DateTime type");
                }

                return value;
            }
            case FileType.DateTimeOffset:
            {
                if (!Utf8Parser.TryParse(propertyValue, out bool value, out _, 'R'))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse DateTimeOffset type");
                }

                return value;
            }
            case FileType.TimeSpan:
            {
                if (!Utf8Parser.TryParse(propertyValue, out TimeSpan value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse TimeSpan type");
                }

                return value;
            }
            case FileType.Guid:
            {
                if (!Utf8Parser.TryParse(propertyValue, out Guid value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse Guid type");
                }

                return value;
            }
            case FileType.Int8:
            {
                if (!Utf8Parser.TryParse(propertyValue, out sbyte value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse sbyte type");
                }

                return value;
            }
            case FileType.UInt8:
            {
                if (!Utf8Parser.TryParse(propertyValue, out byte value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse byte type");
                }

                return value;
            }
            case FileType.Int16:
            {
                if (!Utf8Parser.TryParse(propertyValue, out short value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse short type");
                }

                return value;
            }
            case FileType.UInt16:
            {
                if (!Utf8Parser.TryParse(propertyValue, out ushort value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse short type");
                }

                return value;
            }
            case FileType.Int32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out int value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse int type");
                }

                return value;
            }
            case FileType.UInt32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out uint value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse uint type");
                }

                return value;
            }
            case FileType.Int64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out long value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse long type");
                }

                return value;
            }
            case FileType.UInt64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out ulong value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to parse ulong type");
                }

                return value;
            }
            case FileType.Float32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out float value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to float long type");
                }

                return value;
            }
            case FileType.Float64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out double value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to float double type");
                }

                return value;
            }
            case FileType.Float128:
            {
                if (!Utf8Parser.TryParse(propertyValue, out decimal value, out _))
                {
                    ThrowHelper.ThrowFormatException("Unable to float decimal type");
                }

                return value;
            }
            default:
            {
                return ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(fileType), fileType, "File Type missing implementation");
            }
        }
    }
}
