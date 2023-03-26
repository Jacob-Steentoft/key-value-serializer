using KeyValueSerializer.Models;

// ReSharper disable StringLiteralTypo

namespace KeyValueSerializer.Tests.Unit.Models;

public class TestSerial
{
	[KeyFileName("string")]
	public string? String { get; set; }

    [KeyFileName("strings")]
    public string[]? Strings { get; set; }

    [KeyFileName("bool")]
    public bool? Bool { get; set; }

    [KeyFileName("bools")]
    public bool[]? Bools { get; set; }
    
    [KeyFileName("dateTime")]
    public DateTime? DateTime { get; set; }
    
    [KeyFileName("dateTimes")]
    public DateTime? DateTimes { get; set; }
    
    [KeyFileName("dateTimeOffset")]
    public DateTimeOffset? DateTimeOffset { get; set; }
    
    [KeyFileName("dateTimeOffsets")]
    public DateTimeOffset[]? DateTimeOffsets { get; set; }
    
    [KeyFileName("timeSpan")]
    public TimeSpan? TimeSpan { get; set; }
    
    [KeyFileName("timeSpans")]
    public TimeSpan[]? TimeSpans { get; set; }
    
    [KeyFileName("guid")]
    public Guid? Guid { get; set; }
    
    [KeyFileName("guids")]
    public Guid[]? Guids { get; set; }
    
    [KeyFileName("sbyte")]
    public sbyte? Sbyte { get; set; }
    
    [KeyFileName("sbytes")]
    public sbyte[]? Sbytes { get; set; }
    
    [KeyFileName("byte")]
    public byte? Byte { get; set; }
    
    [KeyFileName("bytes")]
    public byte[]? Bytes { get; set; }
    
    [KeyFileName("short")]
    public short? Short { get; set; }
    
    [KeyFileName("shorts")]
    public short[]? Shorts { get; set; }
    
    [KeyFileName("ushort")]
    public ushort? Ushort { get; set; }
    
    [KeyFileName("ushorts")]
    public ushort[]? Ushorts { get; set; }
    
    [KeyFileName("int")]
    public int? Int { get; set; }
    
    [KeyFileName("ints")]
    public int[]? Ints { get; set; }

    [KeyFileName("uint")]
	public uint? Uint { get; set; }

    [KeyFileName("uints")]
    public uint[]? Units { get; set; }
    
    [KeyFileName("long")]
    public long? Long { get; set; }

    [KeyFileName("longs")]
    public long[]? Longs { get; set; }
    
    [KeyFileName("ulong")]
    public ulong? Ulong { get; set; }

    [KeyFileName("ulongs")]
    public ulong[]? Ulongs { get; set; }
    
    [KeyFileName("float")]
    public float? Float { get; set; }

    [KeyFileName("floats")]
    public float[]? Floats { get; set; }

    [KeyFileName("double")]
    public double? Double { get; set; }

    [KeyFileName("doubles")]
    public double[]? Doubles { get; set; }
    
    [KeyFileName("decimal")]
    public decimal? Decimal { get; set; }

    [KeyFileName("decimals")]
    public decimal[]? Decimals { get; set; }
}
