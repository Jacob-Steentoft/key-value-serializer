namespace KeyValueSerializer.Tests.Unit.TheoryData;

public class IncorrectDateTimeData : TheoryData<string>
{
    public IncorrectDateTimeData()
    {
        const string format = "R";
        
        var first = DateTime.Now;
        Add(first.ToString(format));
        
        var second = DateTime.Today;
        Add(second.ToString(format));
        
        var third = DateTime.MaxValue;
        Add(third.ToString(format));
        
        Add("bingo");
        
        Add("111333.222333");
    }
}
