using System.Text;
using FluentAssertions;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Serialization;
using KeyValueSerializer.Tests.Unit.Models;

namespace KeyValueSerializer.Tests.Unit.Serialization;

public class SerializerTests
{
    [Fact]
    public void Serialize_ServerOptions_CorrectOutput()
    {
        // Arrange
        var testSerial = new TestSerial
        {
            String = "TestString",
            Strings = new[] { "One", "Two", "Three" },
            Bool = true,
            Bools = new[] { true, false, true },
            DateTime = new DateTime(2023, 1, 1),
            DateTimes = new DateTime(2023, 1, 2),
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
            Ints = new int[] { 18, 19, 20 },
            Uint = 21,
            Units = new uint[] { 22, 23, 24 },
            Long = 25,
            Longs = new long[] { 26, 27, 28 },
            Ulong = 29,
            Ulongs = new ulong[] { 30, 31, 32 },
            Float = 33.3f,
            Floats = new float[] { 34.4f, 35.5f, 36.6f },
            Double = 37.7,
            Doubles = new double[] { 38.8, 39.9, 40.0 },
            Decimal = 41.1m,
            Decimals = new decimal[] { 42.2m, 43.3m, 44.4m }
        };

        var options = new KeyValueConfiguration();
        var cache = new KeyValueCache(typeof(TestSerial)); // Assuming you have a KeyValueCache implementation.

        using var memoryStream = new MemoryStream();

        // Act
        Serializer.Serialize(testSerial, memoryStream, cache, options);

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        const string expectedOutput = 
            "string = \"TestString\";\r\n" +
            "strings = {\"One\", \"Two\", \"Three\"};\r\n" +
            "bool = True;\r\n" +
            "bools = {True, False, True};\r\n" +
            "dateTime = 01/01/2023 00:00:00;\r\n" +
            "dateTimes = 01/02/2023 00:00:00;\r\n" +
            "dateTimeOffset = 01/03/2023 00:00:00 +00:00;\r\n" +
            "dateTimeOffsets = {01/04/2023 00:00:00 +00:00, 01/05/2023 00:00:00 +00:00};\r\n" +
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
        
        result.Should().Be(expectedOutput);
    }
}
