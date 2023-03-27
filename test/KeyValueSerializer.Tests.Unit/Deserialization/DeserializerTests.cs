using System.Text;
using FluentAssertions;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Deserialization;
using KeyValueSerializer.Tests.Unit.Models;

namespace KeyValueSerializer.Tests.Unit.Deserialization;

public class DeserializerTests
{
    [Fact]
    public async Task DeserializeStreamAsync_ShouldDeserializeObject_WhenGivenKeyValueStream()
    {
        // Arrange
        var config = new KeyValueConfiguration();
        var newLine = (char)config.ValueEnd + Encoding.UTF8.GetString(config.NewLine);
        var keyValueSeparator = Encoding.UTF8.GetString(new[] { config.Space, config.ValueStart, config.Space });
        var stringSeparator = (char)config.StringSeparator;
        var arrayStart = (char)config.ArrayStart;
        var arraySeparator = Encoding.UTF8.GetString(new[] { config.ArraySeparator, config.Space });
        var arrayEnd = (char)config.ArrayEnd;
        var stringEscape = Encoding.UTF8.GetString(new[] { config.StringIgnoreCharacter, config.StringSeparator });

        var input =
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


        var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var cache = new KeyValueCache(typeof(TestSerial));


        // Act
        var result =
            await Deserializer.DeserializeStreamAsync<TestSerial>(stream, cache, config, CancellationToken.None);

        // Assert
        result.String.Should().Be("TestString");
        result.Strings.Should().Equal("One", "Two", $"Th{stringEscape}ree");
        result.Bool.Should().BeTrue();
        result.Bools.Should().Equal(true, false, true);
        result.DateTime.Should().Be(new DateTime(2023, 1, 1));
        result.DateTimes.Should().Equal(
            new DateTime(2023, 1, 2),
            new DateTime(2022, 1, 2),
            new DateTime(2021, 1, 2));
        result.DateTimeOffset.Should().Be(new DateTimeOffset(new DateTime(2023, 1, 3), TimeSpan.Zero));
        result.DateTimeOffsets.Should().Equal(
            new DateTimeOffset(new DateTime(2023, 1, 4), TimeSpan.Zero),
            new DateTimeOffset(new DateTime(2023, 1, 5), TimeSpan.Zero)
        );
        result.TimeSpan.Should().Be(TimeSpan.FromHours(1));
        result.TimeSpans.Should().Equal(
            TimeSpan.FromHours(2),
            TimeSpan.FromHours(3)
        );
        result.Guid.Should().Be(Guid.Parse("12345678-abcd-1234-abcd-1234567890ab"));
        result.Guids.Should().Equal(
            Guid.Parse("22345678-abcd-1234-abcd-1234567890ab"),
            Guid.Parse("32345678-abcd-1234-abcd-1234567890ab")
        );
        result.Sbyte.Should().Be(1);
        result.Sbytes.Should().Equal(2, 3, 4);
        result.Byte.Should().Be(5);
        result.Bytes.Should().Equal(6, 7, 8);
        result.Short.Should().Be(9);
        result.Shorts.Should().Equal(10, 11, 12);
        result.Ushort.Should().Be(13);
        result.Ushorts.Should().Equal(14, 15, 16);
        result.Int.Should().Be(17);
        result.Ints.Should().Equal(18, 19, 20);
        result.Uint.Should().Be(21);
        result.Units.Should().Equal(22, 23, 24);
        result.Long.Should().Be(25L);
        result.Longs.Should().Equal(26, 27, 28);
        result.Ulong.Should().Be(29UL);
        result.Ulongs.Should().Equal(30, 31, 32);
        result.Float.Should().Be(33.3f);
        result.Floats.Should().Equal(34.4f, 35.5f, 36.6f);
        result.Double.Should().Be(37.7);
        result.Doubles.Should().Equal(38.8, 39.9, 40);
        result.Decimal.Should().Be(41.1m);
        result.Decimals.Should().Equal(42.2m, 43.3m, 44.4m);
    }
}
