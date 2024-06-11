using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using Xunit;

namespace NexYamlTest;
public class BaseFormatterTesting
{
    [Fact]
    public void T()
    {
        var x = new BaseFormatterTest()
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
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(x);
        var d = YamlSerializer.Deserialize<BaseFormatterTest>(s);
        // Assert
        Assert.Equal(x.IntField, d.IntField);

        Assert.Equal(x.FloatField, d.FloatField);

        Assert.Equal(x.DoubleField, d.DoubleField);

        Assert.Equal(x.DecimalField, d.DecimalField);
        Assert.Equal(x.CharField, d.CharField);

        Assert.Equal(x.BoolField, d.BoolField);

        Assert.Equal(x.ByteField, d.ByteField);

        Assert.Equal(x.SByteField, d.SByteField);

        Assert.Equal(x.ShortField, d.ShortField);

        Assert.Equal(x.UShortField, d.UShortField);

        Assert.Equal(x.UIntField, d.UIntField);

        Assert.Equal(x.LongField, d.LongField);

        Assert.Equal(x.ULongField, d.ULongField);

        // Properties
        Assert.Equal(x.IntProperty, d.IntProperty);

        Assert.Equal(x.FloatProperty, d.FloatProperty);

        Assert.Equal(x.DoubleProperty, d.DoubleProperty);

        Assert.Equal(x.DecimalProperty, d.DecimalProperty);

        Assert.Equal(x.CharProperty, d.CharProperty);

        Assert.Equal(x.BoolProperty, d.BoolProperty);

        Assert.Equal(x.ByteProperty, d.ByteProperty);

        Assert.Equal(x.SByteProperty, d.SByteProperty);

        Assert.Equal(x.ShortProperty, d.ShortProperty);

        Assert.Equal(x.UShortProperty, d.UShortProperty);

        Assert.Equal(x.UIntProperty, d.UIntProperty);

        Assert.Equal(x.LongProperty, d.LongProperty);

        Assert.Equal(x.ULongProperty, d.ULongProperty);
        Assert.Equal(x.Time, d.Time);

    }    
    [Fact]
    public void BaseFormatterNullable()
    {
        var x = new BaseFormatterTest()
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
            GuidProperty = Guid.NewGuid(),
            LongProperty = 9223372036854775807,
            Time = new TimeSpan(10),
            ULongProperty = 18446744073709551614,
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(x);
        var d = YamlSerializer.Deserialize<BaseFormatterTest>(s);
        // Assert
        Assert.Equal(x.IntField, d.IntField);
        
        Assert.Equal(x.GuidField, d.GuidField);

        Assert.Equal(x.GuidProperty, d.GuidProperty);

        Assert.Equal(x.FloatField, d.FloatField);

        Assert.Equal(x.DoubleField, d.DoubleField);

        Assert.Equal(x.DecimalField, d.DecimalField);
        Assert.Equal(x.CharField, d.CharField);

        Assert.Equal(x.BoolField, d.BoolField);

        Assert.Equal(x.ByteField, d.ByteField);

        Assert.Equal(x.SByteField, d.SByteField);

        Assert.Equal(x.ShortField, d.ShortField);

        Assert.Equal(x.UShortField, d.UShortField);

        Assert.Equal(x.UIntField, d.UIntField);

        Assert.Equal(x.LongField, d.LongField);

        Assert.Equal(x.ULongField, d.ULongField);

        // Properties
        Assert.Equal(x.IntProperty, d.IntProperty);

        Assert.Equal(x.FloatProperty, d.FloatProperty);

        Assert.Equal(x.DoubleProperty, d.DoubleProperty);

        Assert.Equal(x.DecimalProperty, d.DecimalProperty);

        Assert.Equal(x.CharProperty, d.CharProperty);

        Assert.Equal(x.BoolProperty, d.BoolProperty);

        Assert.Equal(x.ByteProperty, d.ByteProperty);

        Assert.Equal(x.SByteProperty, d.SByteProperty);

        Assert.Equal(x.ShortProperty, d.ShortProperty);

        Assert.Equal(x.UShortProperty, d.UShortProperty);

        Assert.Equal(x.UIntProperty, d.UIntProperty);

        Assert.Equal(x.LongProperty, d.LongProperty);

        Assert.Equal(x.ULongProperty, d.ULongProperty);
        Assert.Equal(x.Time, d.Time);

    }
    [Fact]
    public void BaseNullables()
    {
        var x = new BaseFormatterNullable()
        {
            IntField = null,
            FloatField = null,
            DoubleField = null,
            DecimalField = null,
            CharField = null,
            BoolField = null,
            ByteField = null,
            SByteField = null,
            ShortField = null,
            UShortField = null,
            UIntField = null,
            LongField = null,
            ULongField = null,
            IntProperty = null,
            FloatProperty = null,
            DoubleProperty = null,
            DecimalProperty = null,
            CharProperty = null,
            BoolProperty = null,
            ByteProperty = null,
            SByteProperty = null,
            ShortProperty = null,
            UShortProperty = null,
            UIntProperty = null,
            LongProperty = null,
            ULongProperty = null,
            Time = null
        };

        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(x);
        var d = YamlSerializer.Deserialize<BaseFormatterNullable>(s);

        // Assert.Null() for each property to verify they are all initialized to null
        Assert.Null(d.IntField);
        Assert.Null(d.FloatField);
        Assert.Null(d.DoubleField);
        Assert.Null(d.DecimalField);
        Assert.Null(d.CharField);
        Assert.Null(d.BoolField);
        Assert.Null(d.ByteField);
        Assert.Null(d.SByteField);
        Assert.Null(d.ShortField);
        Assert.Null(d.UShortField);
        Assert.Null(d.UIntField);
        Assert.Null(d.LongField);
        Assert.Null(d.ULongField);

        Assert.Null(d.IntProperty);
        Assert.Null(d.FloatProperty);
        Assert.Null(d.DoubleProperty);
        Assert.Null(d.DecimalProperty);
        Assert.Null(d.CharProperty);
        Assert.Null(d.BoolProperty);
        Assert.Null(d.ByteProperty);
        Assert.Null(d.SByteProperty);
        Assert.Null(d.ShortProperty);
        Assert.Null(d.UShortProperty);
        Assert.Null(d.UIntProperty);
        Assert.Null(d.LongProperty);
        Assert.Null(d.ULongProperty);

        Assert.Null(d.Time);
    }
}
