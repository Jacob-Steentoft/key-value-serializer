namespace KeyValueSerializer.Tests.Unit.TheoryData;

public class CorrectDateTimeData : TheoryData<string, DateTime>
{
    public CorrectDateTimeData()
    {
        const string format = "O";
        
        var first = DateTime.Now;
        Add(first.ToString(format), first);
        
        var second = DateTime.Today;
        Add(second.ToString(format), second);
        
        var third = DateTime.MaxValue;
        Add(third.ToString(format), third);
    }
}
