using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public void BaseNullables()
    {
        var x = new BaseFormatNullable()
        {
            NullInt = null
        };

        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(x);
        var d = YamlSerializer.Deserialize<BaseFormatNullable>(s);

        Assert.Null(d.NullInt);
    }
}
