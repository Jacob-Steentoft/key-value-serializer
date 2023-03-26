using System.Text;
using FluentAssertions;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Deserialization;
using KeyValueSerializer.Tests.Unit.Models;

namespace KeyValueSerializer.Tests.Unit.Deserialization;

public class DeserializerTests
{
    [Fact]
    public async Task DeserializeStreamAsync_GivenValidInput_ReturnsCorrectObject()
    {
        // Arrange
        const string input =
            "string = \"TestString\";\r\n" +
            "strings = {\"One\", \"Two\", \"Three\"};\r\n" +
            "bool = True;\r\n" +
            "bools = {True, False, True};\r\n" +
            "dateTime = 2023-01-01T00:00:00.0000000;\n" +
            "dateTimes = 2023-01-02T00:00:00.0000000;\n" +
            "dateTimeOffset = 2023-01-03T00:00:00.0000000+00:00;\n" +
            "dateTimeOffsets = {2023-01-04T00:00:00.0000000+00:00, 2023-01-05T00:00:00.0000000+00:00};\n" +
            "timeSpan = 01:00:00;\r\n" +
            "timeSpans = {02:00:00, 03:00:00};\r\n" +
            "guid = 12345678-abcd-1234-abcd-1234567890ab;\r\n" +
            "guids = {22345678-abcd-1234-abcd-1234567890ab, 32345678-abcd-1234-abcd-1234567890ab};\r\n" +
            "sbyte = 1;\r\n" +
            "sbytes = {2, 3, 4};\r\n" +
            "byte = 5;\r\n" +
            "bytes = {6, 7, 8};\r\n" +
            "short = 9;\r\n" +
            "shorts = {10, 11, 12};\r\n" +
            "ushort = 13;\r\n" +
            "ushorts = {14, 15, 16};\r\n" +
            "int = 17;\r\n" +
            "ints = {18, 19, 20};\r\n" +
            "uint = 21;\r\n" +
            "uints = {22, 23, 24};\r\n" +
            "long = 25;\r\n" +
            "longs = {26, 27, 28};\r\n" +
            "ulong = 29;\r\n" +
            "ulongs = {30, 31, 32};\r\n" +
            "float = 33.3;\r\n" +
            "floats = {34.4, 35.5, 36.6};\r\n" +
            "double = 37.7;\r\n" +
            "doubles = {38.8, 39.9, 40};\r\n" +
            "decimal = 41.1;\r\n" +
            "decimals = {42.2, 43.3, 44.4};\r\n";


        var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

        var cache = new KeyValueCache(typeof(TestSerial));
        var config = new KeyValueConfiguration();

        // Act
        var result =
            await Deserializer.DeserializeStreamAsync<TestSerial>(stream, cache, config, CancellationToken.None);

        // Assert
        result.String.Should().Be("TestString");
        result.Strings.Should().Equal("One", "Two", "Three");
        result.Bool.Should().BeTrue();
        result.Bools.Should().Equal(true, false, true);
        result.DateTime.Should().Be(new DateTime(2023, 1, 1));
        result.DateTimes.Should().Be(new DateTime(2023, 1, 2));
        result.DateTimeOffset.Should().Be(new DateTimeOffset(new DateTime(2023, 1, 3), TimeSpan.Zero));
        result.DateTimeOffsets.Should().Equal(new DateTimeOffset(new DateTime(2023, 1, 4), TimeSpan.Zero), new DateTimeOffset(new DateTime(2023, 1, 5), TimeSpan.Zero));
        result.TimeSpan.Should().Be(TimeSpan.FromHours(1));
        result.TimeSpans.Should().Equal(TimeSpan.FromHours(2), TimeSpan.FromHours(3));
        result.Guid.Should().Be(Guid.Parse("12345678-abcd-1234-abcd-1234567890ab"));
        result.Guids.Should().Equal(Guid.Parse("22345678-abcd-1234-abcd-1234567890ab"), Guid.Parse("32345678-abcd-1234-abcd-1234567890ab"));
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
