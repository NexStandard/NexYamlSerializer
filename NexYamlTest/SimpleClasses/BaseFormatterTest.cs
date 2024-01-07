using Stride.Core;
using System;
namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class BaseFormatterTest
{

    // Fields
    public int IntField;

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

    // Properties
    public int IntProperty { get; set; }

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

    public TimeSpan Time = new();
}
[DataContract]
internal class BaseFormatNullable
{
    public int? NullInt { get; set; }
}