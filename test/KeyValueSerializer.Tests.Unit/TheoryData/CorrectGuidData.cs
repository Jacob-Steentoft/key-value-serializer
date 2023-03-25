namespace KeyValueSerializer.Tests.Unit.TheoryData;

public class CorrectGuidData : TheoryData<string, Guid>
{
    public CorrectGuidData()
    {
        const string format = "D";

        for (var i = 0; i < 5; i++)
        {
            var guid = Guid.NewGuid();
            Add(guid.ToString(format), guid);
        }
    }
}
