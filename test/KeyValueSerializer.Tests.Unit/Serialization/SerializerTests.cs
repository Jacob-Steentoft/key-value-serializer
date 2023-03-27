using System.Text;
using FluentAssertions;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Serialization;
using KeyValueSerializer.Tests.Unit.Models;

namespace KeyValueSerializer.Tests.Unit.Serialization;

public class SerializerTests
{
    [Fact]
    public void Serialize_ShouldSerializeString_WhenGivenObject()
    {
        // Arrange
        var testSerial = new TestSerial
        {
            String = "TestString",
            Strings = new[] { "One", "Two", "Th\\\"ree" },
            Bool = true,
            Bools = new[] { true, false, true },
            DateTime = new DateTime(2023, 1, 1),
            DateTimes = new[]
            {
                new DateTime(2023, 1, 2),
                new DateTime(2022, 1, 2),
                new DateTime(2021, 1, 2)
            },
            DateTimeOffset = new DateTimeOffset(new DateTime(2023, 1, 3), TimeSpan.Zero),
            DateTimeOffsets = new[]
            {
                new DateTimeOffset(new DateTime(2023, 1, 4), TimeSpan.Zero),
                new DateTimeOffset(new DateTime(2023, 1, 5), TimeSpan.Zero)
            },
            TimeSpan = TimeSpan.FromHours(1),
            TimeSpans = new[] { TimeSpan.FromHours(2), TimeSpan.FromHours(3) },
            Guid = Guid.Parse("12345678-abcd-1234-abcd-1234567890ab"),
            Guids = new[]
            {
                Guid.Parse("22345678-abcd-1234-abcd-1234567890ab"),
                Guid.Parse("32345678-abcd-1234-abcd-1234567890ab")
            },
            Sbyte = 1,
            Sbytes = new sbyte[] { 2, 3, 4 },
            Byte = 5,
            Bytes = new byte[] { 6, 7, 8 },
            Short = 9,
            Shorts = new short[] { 10, 11, 12 },
            Ushort = 13,
            Ushorts = new ushort[] { 14, 15, 16 },
            Int = 17,
            Ints = new[] { 18, 19, 20 },
            Uint = 21U,
            Units = new[] { 22U, 23U, 24U },
            Long = 25L,
            Longs = new[] { 26L, 27L, 28L },
            Ulong = 29UL,
            Ulongs = new[] { 30UL, 31UL, 32UL },
            Float = 33.3f,
            Floats = new[] { 34.4f, 35.5f, 36.6f },
            Double = 37.7,
            Doubles = new[] { 38.8, 39.9, 40.0 },
            Decimal = 41.1m,
            Decimals = new[] { 42.2m, 43.3m, 44.4m }
        };

        var config = new KeyValueConfiguration();
        var cache = new KeyValueCache(typeof(TestSerial));
        using var memoryStream = new MemoryStream();

        // Act
        Serializer.Serialize(testSerial, memoryStream, cache, config);

        // Assert
        var newLine = (char)config.ValueEnd + Encoding.UTF8.GetString(config.NewLine);
        var keyValueSeparator = Encoding.UTF8.GetString(new[] { config.Space, config.ValueStart, config.Space });
        var stringSeparator = (char)config.StringSeparator;
        var arrayStart = (char)config.ArrayStart;
        var arraySeparator = Encoding.UTF8.GetString(new[] { config.ArraySeparator, config.Space });
        var arrayEnd = (char)config.ArrayEnd;
        var stringEscape = Encoding.UTF8.GetString(new[] { config.StringIgnoreCharacter, config.StringSeparator });

        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        var expectedOutput =
            $"string{keyValueSeparator}{stringSeparator}TestString{stringSeparator}{newLine}" +
            $"strings{keyValueSeparator}{arrayStart}{stringSeparator}One{stringSeparator}{arraySeparator}{stringSeparator}Two{stringSeparator}{arraySeparator}{stringSeparator}Th{stringEscape}ree{stringSeparator}{arrayEnd}{newLine}" +
            $"bool{keyValueSeparator}True{newLine}" +
            $"bools{keyValueSeparator}{arrayStart}True{arraySeparator}False{arraySeparator}True{arrayEnd}{newLine}" +
            $"dateTime{keyValueSeparator}2023-01-01T00:00:00.0000000{newLine}" +
            $"dateTimes{keyValueSeparator}{arrayStart}2023-01-02T00:00:00.0000000{arraySeparator}2022-01-02T00:00:00.0000000{arraySeparator}2021-01-02T00:00:00.0000000{arrayEnd}{newLine}" +
            $"dateTimeOffset{keyValueSeparator}2023-01-03T00:00:00.0000000+00:00{newLine}" +
            $"dateTimeOffsets{keyValueSeparator}{arrayStart}2023-01-04T00:00:00.0000000+00:00{arraySeparator}2023-01-05T00:00:00.0000000+00:00{arrayEnd}{newLine}" +
            $"timeSpan{keyValueSeparator}01:00:00{newLine}" +
            $"timeSpans{keyValueSeparator}{arrayStart}02:00:00{arraySeparator}03:00:00{arrayEnd}{newLine}" +
            $"guid{keyValueSeparator}12345678-abcd-1234-abcd-1234567890ab{newLine}" +
            $"guids{keyValueSeparator}{arrayStart}22345678-abcd-1234-abcd-1234567890ab{arraySeparator}32345678-abcd-1234-abcd-1234567890ab{arrayEnd}{newLine}" +
            $"sbyte{keyValueSeparator}1{newLine}" +
            $"sbytes{keyValueSeparator}{arrayStart}2{arraySeparator}3{arraySeparator}4{arrayEnd}{newLine}" +
            $"byte{keyValueSeparator}5{newLine}" +
            $"bytes{keyValueSeparator}{arrayStart}6{arraySeparator}7{arraySeparator}8{arrayEnd}{newLine}" +
            $"short{keyValueSeparator}9{newLine}" +
            $"shorts{keyValueSeparator}{arrayStart}10{arraySeparator}11{arraySeparator}12{arrayEnd}{newLine}" +
            $"ushort{keyValueSeparator}13{newLine}" +
            $"ushorts{keyValueSeparator}{arrayStart}14{arraySeparator}15{arraySeparator}16{arrayEnd}{newLine}" +
            $"int{keyValueSeparator}17{newLine}" +
            $"ints{keyValueSeparator}{arrayStart}18{arraySeparator}19{arraySeparator}20{arrayEnd}{newLine}" +
            $"uint{keyValueSeparator}21{newLine}" +
            $"uints{keyValueSeparator}{arrayStart}22{arraySeparator}23{arraySeparator}24{arrayEnd}{newLine}" +
            $"long{keyValueSeparator}25{newLine}" +
            $"longs{keyValueSeparator}{arrayStart}26{arraySeparator}27{arraySeparator}28{arrayEnd}{newLine}" +
            $"ulong{keyValueSeparator}29{newLine}" +
            $"ulongs{keyValueSeparator}{arrayStart}30{arraySeparator}31{arraySeparator}32{arrayEnd}{newLine}" +
            $"float{keyValueSeparator}33.3{newLine}" +
            $"floats{keyValueSeparator}{arrayStart}34.4{arraySeparator}35.5{arraySeparator}36.6{arrayEnd}{newLine}" +
            $"double{keyValueSeparator}37.7{newLine}" +
            $"doubles{keyValueSeparator}{arrayStart}38.8{arraySeparator}39.9{arraySeparator}40{arrayEnd}{newLine}" +
            $"decimal{keyValueSeparator}41.1{newLine}" +
            $"decimals{keyValueSeparator}{arrayStart}42.2{arraySeparator}43.3{arraySeparator}44.4{arrayEnd}{newLine}";

        result.Should().Be(expectedOutput);
    }
}
