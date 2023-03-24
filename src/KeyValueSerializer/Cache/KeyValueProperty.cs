using Fasterflect;
using KeyValueSerializer.Models;

namespace KeyValueSerializer.Cache;

internal readonly struct KeyValueProperty
{
	public required byte[] KeyName { get; init; }
    public required bool IsArray { get; init; }
    public required FileType FileType { get; init; }
	public required MemberSetter SetValue { get; init; }
	public required MemberGetter GetValue { get; init; }
}
