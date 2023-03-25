namespace KeyValueSerializer.Tests.Unit.TheoryData;

public class CorrectTimeSpanData : TheoryData<string, TimeSpan>
{
    public CorrectTimeSpanData()
    {
        var maxValue = TimeSpan.MaxValue;
        const string format = "c";
        
        Add(maxValue.ToString(format), maxValue);

        var minValue = TimeSpan.MinValue;
        Add(minValue.ToString(format), minValue);

        var zeroValue = TimeSpan.Zero;
        Add(zeroValue.ToString(format), zeroValue);

        var customValue = new TimeSpan(4, 5, 6, 7);
        Add(customValue.ToString(format), customValue);
    }
}
