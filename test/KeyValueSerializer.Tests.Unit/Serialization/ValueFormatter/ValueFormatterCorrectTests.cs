using System.Globalization;
using System.IO.Pipelines;
using System.Text;
using FluentAssertions;
using KeyValueSerializer.Models;
using KeyValueSerializer.Serialization;

namespace KeyValueSerializer.Tests.Unit.ValueFormatter;

public class ValueFormatterCorrectTests
{
    [Theory]
    [InlineData("hæst", '"')]
    [InlineData("testing", '8')]
    public void WritePropertyValueAndAdvance_ShouldSerializeString_WhenGivenString(string input, char stringSeparator)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var config = new KeyValueConfiguration { StringSeparator = (byte)stringSeparator };

        // Act
        pipeWriter.WritePropertyValueAndAdvance(input, config, FileType.String);
        pipeWriter.Complete();
        var array = memoryStream.ToArray();
        var processedString = Encoding.UTF8.GetString(array);

        // Assert
        processedString.Should().Be($"{stringSeparator}{input}{stringSeparator}");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void WritePropertyValueAndAdvance_Boolean_WritesCorrectly(bool boolValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = boolValue.ToString();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(boolValue, options, FileType.Boolean);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData("6a8b0f55-6a47-41c7-92d3-3e7a8a1b0f7b")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void WritePropertyValueAndAdvance_Guid_WritesCorrectly(string guidValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(Guid.Parse(guidValue), options, FileType.Guid);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(guidValue);
    }

    [Theory]
    [InlineData(-128)]
    [InlineData(0)]
    [InlineData(127)]
    public void WritePropertyValueAndAdvance_Int8_WritesCorrectly(sbyte int8Value)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = int8Value.ToString();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(int8Value, options, FileType.Int8);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(128)]
    [InlineData(255)]
    public void WritePropertyValueAndAdvance_UInt8_WritesCorrectly(byte uint8Value)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = uint8Value.ToString();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(uint8Value, options, FileType.UInt8);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData((short)12345)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    public void WritePropertyValueAndAdvance_Int16_CorrectlyWritesValue(short testValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(testValue, options, FileType.Int16);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(testValue.ToString());
    }

    [Theory]
    [InlineData((ushort)12345)]
    [InlineData(ushort.MinValue)]
    [InlineData(ushort.MaxValue)]
    public void WritePropertyValueAndAdvance_UInt16_CorrectlyWritesValue(ushort testValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(testValue, options, FileType.UInt16);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(testValue.ToString());
    }

    [Theory]
    [InlineData(12345678)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void WritePropertyValueAndAdvance_Int_CorrectlyWritesValue(int testValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(testValue, options, FileType.Int32);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(testValue.ToString());
    }

    [Theory]
    [InlineData(12345678U)]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue)]
    public void WritePropertyValueAndAdvance_UInt_CorrectlyWritesValue(uint testValue)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(testValue, options, FileType.UInt32);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(testValue.ToString());
    }

    [Theory]
    [InlineData(long.MinValue, "-9223372036854775808")]
    [InlineData(0L, "0")]
    [InlineData(long.MaxValue, "9223372036854775807")]
    public void WritePropertyValueAndAdvance_Int64_CorrectCases(long inputValue, string expectedOutput)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(inputValue, options, FileType.Int64);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(ulong.MinValue, "0")]
    [InlineData(1UL, "1")]
    [InlineData(ulong.MaxValue, "18446744073709551615")]
    public void WritePropertyValueAndAdvance_UInt64_CorrectCases(ulong inputValue, string expectedOutput)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(inputValue, options, FileType.UInt64);
        pipeWriter.Complete();

        // Assert
        var result = Encoding.UTF8.GetString(memoryStream.ToArray());
        result.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(float.MaxValue)]
    [InlineData(float.MinValue)]
    [InlineData(-3.14159)]
    public void WritePropertyValueAndAdvance_Float_WritesCorrectly(float input)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = input.ToString("G", CultureInfo.InvariantCulture);

        // Act
        pipeWriter.WritePropertyValueAndAdvance(input, options, FileType.Float32);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(double.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(-3.14159)]
    public void WritePropertyValueAndAdvance_Double_WritesCorrectly(double input)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = input.ToString("G", CultureInfo.InvariantCulture);

        // Act
        pipeWriter.WritePropertyValueAndAdvance(input, options, FileType.Float64);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }

    [Theory]
    [InlineData(7922816251426433759d)]
    [InlineData(-7922816251426433759d)]
    [InlineData(-3.14159d)]
    public void WritePropertyValueAndAdvance_Decimal_WritesCorrectly(decimal input)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();
        var expectedOutput = input.ToString("G", CultureInfo.InvariantCulture);

        // Act
        pipeWriter.WritePropertyValueAndAdvance(input, options, FileType.Float128);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().Be(expectedOutput);
    }
}
