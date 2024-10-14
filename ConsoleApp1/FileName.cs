using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using NexVYaml;
using Stride.Core;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class BaseFormatterTest
{

    // Fields
    public int IntField;

    public string StringField = "";

    public float FloatField;

    public double DoubleField;

    public decimal DecimalField;

    public char CharField;

    public bool BoolField;

    public byte ByteField;

    public sbyte SByteField;

    public short ShortField;

    public ushort UShortField;

    public uint UIntField;

    public long LongField;
    public ulong ULongField;

    public Guid GuidField = new();

    // Properties
    public int IntProperty { get; set; }

    public string StringProperty { get; set; } = string.Empty;

    public float FloatProperty { get; set; }

    public double DoubleProperty { get; set; }

    public decimal DecimalProperty { get; set; }

    public char CharProperty { get; set; }

    public bool BoolProperty { get; set; }

    public byte ByteProperty { get; set; }

    public sbyte SByteProperty { get; set; }

    public short ShortProperty { get; set; }

    public ushort UShortProperty { get; set; }

    public uint UIntProperty { get; set; }

    public long LongProperty { get; set; }

    public ulong ULongProperty { get; set; }

    public Guid GuidProperty { get; set; }
    public TimeSpan Time = new();
    public Uri Uri { get; set; } = new Uri("https://www.example.com/path?query=example#fragment");
}
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(BaseFormatterTest))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}
[MemoryDiagnoser]
public class BenchmarkSerialization
{
    private readonly BaseFormatterTest _testObject = new BaseFormatterTest()
    {
        // Assigning example values
        IntField = 42,

        FloatField = 3.14f,

        DoubleField = 2.718,
        DecimalField = 123.456m,

        CharField = 'A',

        BoolField = true,

        ByteField = 255,

        SByteField = -128,
        ShortField = -32768,

        UShortField = 65535,

        UIntField = 4294967295,

        ULongField = 18446744073709551615,

        // Assigning example values for properties
        IntProperty = 100,

        FloatProperty = 1.23f,

        DoubleProperty = 4.567,

        DecimalProperty = 789.012m,

        CharProperty = 'B',

        BoolProperty = false,

        ByteProperty = 127,

        SByteProperty = 127,

        ShortProperty = 32767,

        UShortProperty = 12345,

        UIntProperty = 987654321,

        LongProperty = 9223372036854775807,
        Time = new TimeSpan(10),
        ULongProperty = 18446744073709551614,
        //    NullableULongProperty = null,
    };
    // Serialize using System.Text.Json 1000 times
    [Benchmark]
    public void Serialize1000Times()
    {

            var json = JsonSerializer.Serialize(_testObject,SourceGenerationContext.Default.BaseFormatterTest);
        var d = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.BaseFormatterTest);
        
    }

    // Deserialize using System.Text.Json 1000 times
    [Benchmark]
    public void YamlSerialize()
    {

            var obj = YamlSerializer.SerializeToString(_testObject);
        var d = YamlSerializer.Deserialize<BaseFormatterTest>(obj);

    }
}
