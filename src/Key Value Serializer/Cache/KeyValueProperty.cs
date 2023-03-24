using Fasterflect;
using Key_Value_Serializer.Models;

namespace Key_Value_Serializer.Cache;

internal readonly struct KeyValueProperty
{
	public required byte[] KeyName { get; init; }
    public required bool IsArray { get; init; }
    public required FileType FileType { get; init; }
	public required MemberSetter SetValue { get; init; }
	public required MemberGetter GetValue { get; init; }
}
