using System.Buffers.Text;
using System.Text;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.HighPerformance.Buffers;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Deserialization;

internal static class ValueParser
{
    public static void SetProperty(object buildObject, KeyValueProperty property,
        scoped ReadOnlySpan<byte> propertyValue, KeyValueConfiguration config)
    {
        var objectValue = ParseFileProperty(propertyValue, property.FileType, config);
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

    public static int IndexOfArrayEnd(scoped ReadOnlySpan<byte> buffer, KeyValueConfiguration config,
        out int arraySize)
    {
        arraySize = 1;
        const int invalidLength = -1;
        for (var index = 0; index < buffer.Length; index++)
        {
            var bufferByte = buffer[index];

            if (bufferByte == config.StringSeparator)
            {
                var endStringIndex = StringEndIndex(buffer.Slice(index), config);

                if (endStringIndex == invalidLength)
                {
                    return invalidLength;
                }

                index += endStringIndex;
                continue;
            }

            if (bufferByte == config.ArraySeparator)
            {
                arraySize++;
                continue;
            }

            if (bufferByte == config.ArrayEnd)
            {
                return index + 1;
            }
        }

        return invalidLength;
    }

    private static void SetArray<T>(object buildObject, KeyValueProperty property,
        scoped ReadOnlySpan<byte> arrayBytes, KeyValueConfiguration config, int arraySize)
    {
        var array = new T[arraySize];

        for (var index = 0; index < arraySize; index++)
        {
            var itemBytes = GetArrayItem(arrayBytes, config, out var nextIndex);
            array[index] = (T)ParseFileProperty(itemBytes, property.FileType, config);

            arrayBytes = arrayBytes.Slice(nextIndex);
        }

        property.SetValue(buildObject, array);
    }

    private static ReadOnlySpan<byte> GetArrayItem(ReadOnlySpan<byte> buffer, KeyValueConfiguration options,
        out int nextIndex)
    {
        var startIndex = -1;
        for (var index = 0; index < buffer.Length; index++)
        {
            var bufferByte = buffer[index];

            if (bufferByte == options.StringSeparator)
            {
                var stringEndIndex = StringEndIndex(buffer.Slice(index), options);
                if (stringEndIndex == -1)
                {
                    nextIndex = -1;
                    return ReadOnlySpan<byte>.Empty;
                }

                nextIndex = index + stringEndIndex + 1;
                return buffer.Slice(index + 1, stringEndIndex - 1);
            }

            if (bufferByte != options.ArrayStart &&
                bufferByte != options.ArraySeparator &&
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
            return buffer.Slice(startIndex + 1, index - startIndex - 1).TrimStart(options.WhiteSpaces);
        }

        nextIndex = -1;
        return ReadOnlySpan<byte>.Empty;
    }

    private static int StringEndIndex(scoped ReadOnlySpan<byte> buffer, KeyValueConfiguration options)
    {
        var stringSeparatorIndex = 0;
        do
        {
            stringSeparatorIndex++;
            var newIndex = buffer.Slice(stringSeparatorIndex).IndexOf(options.StringSeparator);
            if (newIndex < 0)
            {
                return -1;
            }

            stringSeparatorIndex += newIndex;
        } while (buffer[stringSeparatorIndex - 1] == options.StringIgnoreCharacter);

        return stringSeparatorIndex;
    }

    private static object ParseFileProperty(scoped ReadOnlySpan<byte> propertyValue, FileType fileType,
        KeyValueConfiguration config)
    {
        switch (fileType)
        {
            case FileType.String:
            {
                return StringPool.Shared.GetOrAdd(propertyValue, Encoding.UTF8);
            }
            case FileType.Boolean:
            {
                if (!Utf8Parser.TryParse(propertyValue, out bool value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse bool type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.DateTime:
            {
                if (!Utf8Parser.TryParse(propertyValue, out DateTime value, out _, config.DateTimeFormat))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse DateTime type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.DateTimeOffset:
            {
                if (!Utf8Parser.TryParse(propertyValue, out DateTimeOffset value, out _, config.DateTimeFormat))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse DateTimeOffset type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.TimeSpan:
            {
                if (!Utf8Parser.TryParse(propertyValue, out TimeSpan value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse TimeSpan type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Guid:
            {
                if (!Utf8Parser.TryParse(propertyValue, out Guid value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse Guid type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Int8:
            {
                if (!Utf8Parser.TryParse(propertyValue, out sbyte value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse sbyte type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.UInt8:
            {
                if (!Utf8Parser.TryParse(propertyValue, out byte value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse byte type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Int16:
            {
                if (!Utf8Parser.TryParse(propertyValue, out short value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse short type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.UInt16:
            {
                if (!Utf8Parser.TryParse(propertyValue, out ushort value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse short type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Int32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out int value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse int type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.UInt32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out uint value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse uint type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Int64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out long value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse long type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.UInt64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out ulong value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse ulong type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Float32:
            {
                if (!Utf8Parser.TryParse(propertyValue, out float value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse float type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Float64:
            {
                if (!Utf8Parser.TryParse(propertyValue, out double value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse double type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            case FileType.Float128:
            {
                if (!Utf8Parser.TryParse(propertyValue, out decimal value, out _))
                {
                    ThrowHelper.ThrowFormatException(
                        $"Unable to parse decimal type: '{Encoding.UTF8.GetString(propertyValue)}'");
                }

                return value;
            }
            default:
            {
                return ThrowHelper.ThrowArgumentOutOfRangeException<object>(nameof(fileType), fileType,
                    "File Type missing implementation for file parser");
            }
        }
    }
}
