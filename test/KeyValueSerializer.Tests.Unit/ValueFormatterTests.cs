namespace KeyValueSerializer.Tests.Unit;

public class ValueFormatterTests
{
    [Theory]
    [InlineData("hæst","\"hæst\"")]
    public void WritePropertyValueAndAdvance_ShouldSerializeString_WhenGivenString(string input, string expect)
    {
        
    }
}
