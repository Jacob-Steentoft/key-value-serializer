namespace KeyValueSerializer.Models;

[AttributeUsage(AttributeTargets.Property)]
public sealed class KeyFileName : Attribute
{
	public KeyFileName(string name)
	{
		Name = name;
    }

	public string Name { get; }
}
