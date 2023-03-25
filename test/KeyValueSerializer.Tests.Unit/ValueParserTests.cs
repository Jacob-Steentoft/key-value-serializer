using System.Text;
using FluentAssertions;
using KeyValueSerializer.Cache;
using KeyValueSerializer.Deserialization;
using KeyValueSerializer.Tests.Unit.Models;
using KeyValueSerializer.Tests.Unit.TheoryData;

namespace KeyValueSerializer.Tests.Unit;

public class ValueParserTests
{
    private readonly KeyValueCache _cache;
    private readonly ArmaServerOptions _options;

    public ValueParserTests()
    {
        _cache = new KeyValueCache(typeof(ArmaServerOptions));
        _options = new ArmaServerOptions();
    }

    // String testing
    [Theory]
    [InlineData("bingo")]
    [InlineData("1")]
    [InlineData("")]
    [InlineData("...........")]
    // ReSharper disable twice StringLiteralTypo
    [InlineData("Rød grød med fløde")]
    [InlineData("""
                                    \"\
                                  """)]
    public void SetProperty_ShouldSerializeString_WhenGivenStringPropertyAndStringValue(string inputAndResult)
    {
        // Arrange
        var key = "string"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        var value = Encoding.UTF8.GetBytes(inputAndResult);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.String.Should().Be(inputAndResult);
    }

    // Bool testing
    [Theory]
    [InlineData("false", false)]
    [InlineData("true", true)]
    [InlineData("False", false)]
    [InlineData("True", true)]
    [InlineData("true         ", true)]
    [InlineData("falseNot", false)]
    public void SetProperty_ShouldSerializeBool_WhenGivenBoolPropertyAndBoolValue(string input, bool result)
    {
        // Arrange
        var key = "bool"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Bool.Should().Be(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1")]
    [InlineData("hello")]
    [InlineData("tru e")]
    [InlineData(" false")]
    [InlineData(" false ")]
    public void SetProperty_ShouldThrow_WhenGivenBoolPropertyAndIncorrectValue(string input)
    {
        // Arrange
        var key = "bool"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }

    // DateTime testing
    [Theory]
    [ClassData(typeof(CorrectDateTimeData))]
    public void SetProperty_ShouldSerializeDateTime_WhenGivenDateTimePropertyAndDateTimeValue(string input, DateTime expect)
    {
        // Arrange
        var key = "dateTime"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.DateTime.Should().Be(expect);
    }
    
    [Theory]
    [ClassData(typeof(IncorrectDateTimeData))]
    public void SetProperty_ShouldThrow_WhenGivenDateTimePropertyAndIncorrectValue(string input)
    {
        // Arrange
        var key = "dateTime"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // DateTimeOffset testing
    [Theory]
    [ClassData(typeof(CorrectDateTimeOffsetData))]
    public void SetProperty_ShouldSerializeDateTimeOffset_WhenGivenDateTimeOffsetPropertyAndDateTimeOffsetValue(string input, DateTimeOffset expect)
    {
        // Arrange
        var key = "dateTimeOffset"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.DateTimeOffset.Should().Be(expect);
    }
    
    [Theory]
    [ClassData(typeof(IncorrectDateTimeData))]
    public void SetProperty_ShouldThrow_WhenGivenDateTimeOffsetPropertyAndIncorrectValue(string input)
    {
        // Arrange
        var key = "dateTimeOffset"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // TimeSpan testing
    [Theory]
    [ClassData(typeof(CorrectTimeSpanData))]
    public void SetProperty_ShouldSerializeTimeSpan_WhenGivenDateTimeOffsetPropertyAndDateTimeOffsetValue(string input, TimeSpan expect)
    {
        // Arrange
        var key = "timeSpan"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.TimeSpan.Should().Be(expect);
    }
    
    [Theory]
    [InlineData(".1990")]
    [InlineData("Haha")]
    [InlineData("Åse")]
    [InlineData("1773.69.9999")]
    public void SetProperty_ShouldThrow_WhenGivenTimeSpanPropertyAndIncorrectValue(string input)
    {
        // Arrange
        var key = "timeSpan"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // Guid testing
    [Theory]
    [ClassData(typeof(CorrectGuidData))]
    public void SetProperty_ShouldSerializeGuid_WhenGivenGuidPropertyAndGuidValue(string input, Guid expect)
    {
        // Arrange
        var key = "guid"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Guid.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("{EC883631-3B5A-4B80-BD7A-8A0024817D66}")]
    [InlineData("""
                        "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData("1773.69.9999")]
    public void SetProperty_ShouldThrow_WhenGivenGuidAndIncorrectValue(string input)
    {
        // Arrange
        var key = "guid"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // sbyte testing
    [Theory]
    [InlineData("1",1)]
    [InlineData("100",100)]
    [InlineData("-100",-100)]
    [InlineData("127",sbyte.MaxValue)]
    [InlineData("-128",sbyte.MinValue)]
    public void SetProperty_ShouldSerializeSbyte_WhenGivenSbytePropertyAndSbyteValue(string input, sbyte expect)
    {
        // Arrange
        var key = "sbyte"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Sbyte.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("900")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData("1773.69.9999")]
    [InlineData("-300")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenSbyteAndIncorrectValue(string input)
    {
        // Arrange
        var key = "sbyte"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // byte testing
    [Theory]
    [InlineData("1",1)]
    [InlineData("100",100)]
    [InlineData("123",123)]
    [InlineData("255",byte.MaxValue)]
    [InlineData("0",byte.MinValue)]
    public void SetProperty_ShouldSerializeByte_WhenGivenBytePropertyAndByteValue(string input, byte expect)
    {
        // Arrange
        var key = "byte"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Byte.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("900")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("-1")]
    [InlineData("Åse")]
    [InlineData("1773.69.9999")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenByteAndIncorrectValue(string input)
    {
        // Arrange
        var key = "byte"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // short testing
    [Theory]
    [InlineData("-1",-1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("32767",short.MaxValue)]
    [InlineData("-32768",short.MinValue)]
    public void SetProperty_ShouldSerializeShort_WhenGivenShortPropertyAndShortValue(string input, short expect)
    {
        // Arrange
        var key = "short"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Short.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-32769")]
    [InlineData("32768")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenShortAndIncorrectValue(string input)
    {
        // Arrange
        var key = "short"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // ushort testing
    [Theory]
    [InlineData("1",1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("65535",ushort.MaxValue)]
    [InlineData("0",ushort.MinValue)]
    public void SetProperty_ShouldSerializeUshort_WhenGivenUshortPropertyAndUshortValue(string input, ushort expect)
    {
        // Arrange
        var key = "ushort"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Ushort.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-1")]
    [InlineData("65536")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenUshortAndIncorrectValue(string input)
    {
        // Arrange
        var key = "ushort"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // int testing
    [Theory]
    [InlineData("-1",-1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("2147483647",int.MaxValue)]
    [InlineData("-2147483648",int.MinValue)]
    public void SetProperty_ShouldSerializeInt_WhenGivenIntPropertyAndIntValue(string input, int expect)
    {
        // Arrange
        var key = "int"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Int.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-2147483649")]
    [InlineData("2147483648")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenIntAndIncorrectValue(string input)
    {
        // Arrange
        var key = "int"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // uint testing
    [Theory]
    [InlineData("1",1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("4294967295",uint.MaxValue)]
    [InlineData("0",uint.MinValue)]
    public void SetProperty_ShouldSerializeUint_WhenGivenUintPropertyAndUintValue(string input, uint expect)
    {
        // Arrange
        var key = "uint"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Uint.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-1")]
    [InlineData("4294967296")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenUintAndIncorrectValue(string input)
    {
        // Arrange
        var key = "uint"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // long testing
    [Theory]
    [InlineData("-1",-1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("9223372036854775807",long.MaxValue)]
    [InlineData("-9223372036854775808",long.MinValue)]
    public void SetProperty_ShouldSerializeLong_WhenGivenLongPropertyAndLongValue(string input, long expect)
    {
        // Arrange
        var key = "long"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Long.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-9223372036854775809")]
    [InlineData("9223372036854775808")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenLongAndIncorrectValue(string input)
    {
        // Arrange
        var key = "long"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // ulong testing
    [Theory]
    [InlineData("1",1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("18446744073709551615",ulong.MaxValue)]
    [InlineData("0",ulong.MinValue)]
    public void SetProperty_ShouldSerializeUlong_WhenGivenUlongPropertyAndUlongValue(string input, ulong expect)
    {
        // Arrange
        var key = "ulong"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Ulong.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("-1")]
    [InlineData("18446744073709551616")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData(".100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenUlongAndIncorrectValue(string input)
    {
        // Arrange
        var key = "ulong"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // float testing
    [Theory]
    [InlineData("-1",-1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("3.4028235E+38",float.MaxValue)]
    [InlineData("-3.4028235E+38",float.MinValue)]
    public void SetProperty_ShouldSerializeFloat_WhenGivenFloatPropertyAndFloatValue(string input, float expect)
    {
        // Arrange
        var key = "float"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Float.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("infintelylong will be accepeted")]
    [InlineData("prett ymuch any number that starts4.40.28249+49")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData("&100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenFloatAndIncorrectValue(string input)
    {
        // Arrange
        var key = "float"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
    
    // double testing
    [Theory]
    [InlineData("-1",-1)]
    [InlineData("0",0)]
    [InlineData("123",123)]
    [InlineData("3.4028235E+38",float.MaxValue)]
    [InlineData("-3.4028235E+38",float.MinValue)]
    public void SetProperty_ShouldSerializeDouble_WhenGivenDoublePropertyAndDoubleValue(string input, float expect)
    {
        // Arrange
        var key = "float"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);
        
        var value = Encoding.UTF8.GetBytes(input);

        // Act
        ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        _options.Float.Should().Be(expect);
    }
    
    [Theory]
    [InlineData("infintelylong will be accepeted")]
    [InlineData("prett ymuch any number that starts4.40.28249+49")]
    [InlineData("""
                            "F7D2B79C-BE97-42FA-B4F9-D2B3A52B333D"
                        """)]
    [InlineData("Åse")]
    [InlineData("&100")]
    [InlineData(" 100")]
    public void SetProperty_ShouldThrow_WhenGivenDoubleAndIncorrectValue(string input)
    {
        // Arrange
        var key = "float"u8;
        var keyValueProperty = _cache.GetKeyValueProperty(key);

        var value = Encoding.UTF8.GetBytes(input);

        // Act
        var result = () => ValueParser.SetProperty(_options, keyValueProperty, value);

        // Assert
        result.Should().Throw<FormatException>();
    }
}
