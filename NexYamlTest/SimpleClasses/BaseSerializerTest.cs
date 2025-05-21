using NexYaml.Serializers;
using Stride.Core;
using System;
namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class BaseSerializerTest
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
    public DateTimeOffset DateTimeOffset { get; set; } = new();
    public DateTime DateTime { get; set; } = new();
}
[DataContract]
internal class BaseSerializerNullable
{
    // Fields
    public int? IntField { get; set; } = 42;

    public string StringField { get; set; } = "asdf";

    public float? FloatField { get; set; } = 3.14f;

    public double? DoubleField { get; set; } = 2.71828;

    public decimal? DecimalField { get; set; } = 123.456m;

    public char? CharField { get; set; } = 'A';

    public bool? BoolField { get; set; } = true;

    public byte? ByteField { get; set; } = 255;

    public sbyte? SByteField { get; set; } = -128;

    public short? ShortField { get; set; } = -32768;

    public ushort? UShortField { get; set; } = 65535;

    public uint? UIntField { get; set; } = 4294967295;

    public long? LongField { get; set; } = 9223372036854775807;

    public ulong? ULongField { get; set; } = 18446744073709551615;

    // Properties
    public int? IntProperty { get; set; } = 100;

    public float? FloatProperty { get; set; } = 3.5f;

    public double? DoubleProperty { get; set; } = 10239.01;

    public decimal? DecimalProperty { get; set; } = 789.012m;

    public char? CharProperty { get; set; } = 'B';

    public bool? BoolProperty { get; set; } = false;

    public byte? ByteProperty { get; set; } = 128;

    public sbyte? SByteProperty { get; set; } = 127;

    public short? ShortProperty { get; set; } = 32767;

    public ushort? UShortProperty { get; set; } = 54321;

    public uint? UIntProperty { get; set; } = 987654321;

    public long? LongProperty { get; set; } = -12391240291209;

    public ulong? ULongProperty { get; set; } = 12094108541289510239;

    public TimeSpan? Time { get; set; } = new TimeSpan();
    public Guid? GuidProperty { get; set; } = null;
    public Uri? Uri { get; set; } = null;
    public DateTimeOffset? DateTimeOffset { get; set; } = null;
    public DateTime? DateTime { get; set; } = null;
}