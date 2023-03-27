using System.IO.Pipelines;
using System.Text;
using FluentAssertions;
using KeyValueSerializer.Models;
using KeyValueSerializer.Serialization;

namespace KeyValueSerializer.Tests.Unit.Serialization.Formatter;

public class ValueFormatterIncorrectTests
{
    [Theory]
    [InlineData("6a8b0f55-6a47-41c7-92d3-3e7a8a1b0f7b", "00000000-0000-0000-0000-000000000000")]
    [InlineData("00000000-0000-0000-0000-000000000000", "6a8b0f55-6a47-41c7-92d3-3e7a8a1b0f7b")]
    public void WritePropertyValueAndAdvance_Guid_WritesIncorrectly(string guidValue, string incorrectExpectedOutput)
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
        actualOutput.Should().NotBe(incorrectExpectedOutput);
    }

    [Theory]
    [InlineData(-128, 127)]
    [InlineData(0, 1)]
    [InlineData(127, -128)]
    public void WritePropertyValueAndAdvance_Int8_WritesIncorrectly(sbyte int8Value, sbyte incorrectExpectedOutput)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(int8Value, options, FileType.Int8);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().NotBe(incorrectExpectedOutput.ToString());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(128, 127)]
    [InlineData(255, 254)]
    public void WritePropertyValueAndAdvance_UInt8_WritesIncorrectly(byte uint8Value, byte incorrectExpectedOutput)
    {
        // Arrange
        using var memoryStream = new MemoryStream();
        var pipeWriter = PipeWriter.Create(memoryStream);
        var options = new KeyValueConfiguration();

        // Act
        pipeWriter.WritePropertyValueAndAdvance(uint8Value, options, FileType.UInt8);
        pipeWriter.Complete();

        // Assert
        var actualOutput = Encoding.UTF8.GetString(memoryStream.ToArray());
        actualOutput.Should().NotBe(incorrectExpectedOutput.ToString());
    }
}
