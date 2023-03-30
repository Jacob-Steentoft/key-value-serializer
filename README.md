# Key Value Serializer

## About

This project is a performance focused .NET Library for serializing key value files.
These files are commonly found when working with configuring video games or other applications that have human readable setting files that allow users to change the configuration through a text file.

## Mission

Creating a high-performance and low allocation key value UTF8 serializer that can be customized without code changes.

## How to Use

Use the static class ```KeyValueSerializer``` with either the ```DeserializeAsync<T>``` or ```Serialize``` method for which task you want to do.

```C#
using var fileStream = File.Open("mypath", FileMode.Open);
TestSerial myNewObject = await KeyValueSerializer.DeserializeAsync<TestSerial>(fileStream);
```

```C#
var myNewObject = new TestSerial();
using var fileStream = File.Open("mypath", FileMode.OpenOrCreate);
KeyValueSerializer.Serialize(fileStream, myNewObject);
```

If your type property has a name that differs from the file make sure to annotate the property using ```KeyFileName``` attribute.

```C#
[KeyFileName("strings")]
public string[]? Strings { get; set; }
```

## Why

For my personal projects I needed a way to serialize key value files with support for values that could span multiple lines and would deserialize into static objects.
