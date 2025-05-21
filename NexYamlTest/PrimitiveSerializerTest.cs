using NexYaml;
using NexYaml.Core;
using NexYaml.Serializers;
using NexYamlTest.SimpleClasses;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class PrimitiveSerializerTest
{
    [Fact]
    public async Task PrimitiveSerializer_NonNullable()
    {
        var x = new BaseSerializerTest()
        {
            // Assigning example values
            IntField = 42,
            StringField = "#:;",
            FloatField = 3.14f,

            DoubleField = 2.718,
            DecimalField = 123.456m,

            CharField = ':',

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

            CharProperty = '#',

            BoolProperty = false,

            ByteProperty = 127,

            SByteProperty = 127,
            StringProperty = "\n#::##\n\n",
            ShortProperty = 32767,

            UShortProperty = 12345,

            UIntProperty = 987654321,

            LongProperty = 9223372036854775807,
            Time = new TimeSpan(10),
            ULongProperty = 18446744073709551614,
            //    NullableULongProperty = null,
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(x);
        var d = await Yaml.Read<BaseSerializerTest>(s);
        // Assert
        Assert.Equal(x.IntField, d.IntField);

        Assert.Equal(x.FloatField, d.FloatField);

        Assert.Equal(x.DoubleField, d.DoubleField);

        Assert.Equal(x.DecimalField, d.DecimalField);
        Assert.Equal(x.CharField, d.CharField);
        Assert.Equal(x.Uri, d.Uri);
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

        Assert.Equal(x.StringField, d.StringField);
        
        Assert.Equal(x.StringProperty, d.StringProperty);
        Assert.Equal(x.DateTime, d.DateTime);
        Assert.Equal(x.DateTimeOffset, d.DateTimeOffset);
        Assert.Equal(x.Uri, d.Uri);

    }
    [Fact]
    public async Task PrimitiveNullableWithValues()
    {
        var x = new BaseSerializerNullable()
        {
            IntField = 1,
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(x);
        var d = await Yaml.Read<BaseSerializerTest>(s);
        Assert.Equal(x.IntField, d.IntField);
    }
    [Fact]
    public async Task BaseSerializerWithNullables()
    {
        var x = new BaseSerializerTest()
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
        var s = Yaml.Write(x);
        var d = await Yaml.Read<BaseSerializerTest>(s);
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
    public async Task BaseNullables()
    {
        var x = new BaseSerializerNullable()
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
        var s = Yaml.Write(x);
        var d = await Yaml.Read<BaseSerializerNullable>(s);

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
    [Fact]
    public async Task Exception_On_Wrong_Primitive_Type()
    {
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(new Generics<Generics<int>>()
        {
            Value = new()
        });

        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<bool>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<byte>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<char>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<DateTimeOffset>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<DateTime>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<decimal>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<float>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<long>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<double>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<short>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<int>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<sbyte>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<ushort>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<uint>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<ulong>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<Guid>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<TimeSpan>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<Uri>(s));
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<TimeSpan>(s));
    }
}
