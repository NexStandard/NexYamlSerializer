using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using Stride.Core;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYamlSerializer.Serialization.Formatters;
using Silk.NET.OpenXR;
using Irony.Parsing;
namespace NexYamlTest;
public class ComplexTests
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact(Skip = "Unimplemented ICollection handling")]
    public void DoubleInheritedListTest()
    {
        Setup();
        var list = new DoubleInheritedList()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
        };

        var s = YamlSerializer.SerializeToString(list);
        var deserialized = YamlSerializer.Deserialize<DoubleInheritedList>(s);
        Assert.Null(s);
        Assert.Equal(list.Count,deserialized.Count);
        for ( var i = 0; i < list.Count; i++ )
        {
            Assert.Equal(list[i], deserialized[i] );
        }
    }
    [Fact]
    public void SecureModeTest()
    {
        Setup();
        var secureObject = new SecureMode();
        IInSecure inSecure = secureObject;
        var secureMode = new YamlSerializerOptions()
        {
            Resolver = NexYamlSerializerRegistry.Instance,
            SecureMode = true
        };
        var s = YamlSerializer.SerializeToString(secureObject,secureMode);
        var deserialized = YamlSerializer.Deserialize<SecureMode>(s);

        Assert.NotNull(deserialized);
        var insecureSerialize = YamlSerializer.SerializeToString(inSecure, secureMode);
        Assert.Equal("!!null",insecureSerialize.ToString());
        var insecureDeserialize = YamlSerializer.Deserialize<IInSecure>(s, secureMode);
        Assert.Null(insecureDeserialize);
    }
    [Fact]
    public void DynamicGenerics()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClass<int, int>()
        {
            Generic2 = 1,
            Generic = 4
        };
        var s = YamlSerializer.SerializeToString(genericInterface);
        var deserialized = YamlSerializer.Deserialize<IGenericInterface<int, int>>(s);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2 , deserialized.Generic2);
    }
    [Fact]
    public void LessGenericsThanRoot()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClassWithLessParams<int>()
        {
            Generic2 = 1,
            Generic = 4
        };
        var s = YamlSerializer.SerializeToString(genericInterface);
        var deserialized = YamlSerializer.Deserialize<IGenericInterface<int, int>>(s);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void NestedDynamicGenerics()
    {
        Setup();
        IGenericInterface<IGenericInterface<int,int>, int> genericInterface = new GenericImplementedClassWithLessParams<IGenericInterface<int, int>>()
        {
            Generic2 = 1,
            Generic = new GenericImplementedClassWithLessParams<int>()
            {
                Generic2 = 2,
                Generic = 4
            }
        };
        var s = YamlSerializer.SerializeToString(genericInterface);
        var deserialized = YamlSerializer.Deserialize<IGenericInterface<IGenericInterface<int, int>, int>>(s);
        Assert.Equal(genericInterface.Generic.Generic, deserialized.Generic.Generic);
        Assert.Equal(genericInterface.Generic.Generic2, deserialized.Generic.Generic2);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }    [Fact]
    public void NoParamsImplementation()
    {
        Setup();
        IGenericInterface<int,int> genericInterface = new GenericImplementedClassWithNoParams()
        {
            Generic2 = 1,
            Generic = 10
        };
        var s = YamlSerializer.SerializeToString(genericInterface);
        var deserialized = YamlSerializer.Deserialize<IGenericInterface<int, int>>(s);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void InheritedSameGenerics()
    {
        Setup();
        GenericAbstract<int, int> genericInterface = new GenericAbstractImplementation<int,int>()
        {
            TI = 1,
            TI2 = 10
        };
        var s = YamlSerializer.SerializeToString(genericInterface);
        var deserialized = YamlSerializer.Deserialize<GenericAbstractImplementation<int, int>>(s);
        Assert.Equal(genericInterface.Test, deserialized.Test);
    }
    [Fact()]
    public void InheritedNoDatacontractOnAbstractClass()
    {
        Setup();
        GenericAbstract<int, int> abstractObject = new GenericAbstractImlementationLessParams<int>()
        {
            Test = 3
        };
        var s = YamlSerializer.SerializeToString(abstractObject);
        var deserialized = YamlSerializer.Deserialize<GenericAbstractImlementationLessParams<int>>(s);
        Assert.Equal(0, deserialized.Test);
    }
    [Fact()]
    public void InheritedNoDatacontractOnAbstractClassWithDataContract()
    {
        Setup();
        GenericAbstractWithDataContract<int, int> abstractObject = new GenericAbstractImlementationLessParamsDataContract<int>()
        {
            Test = 3
        };
        var s = YamlSerializer.SerializeToString(abstractObject);
        var deserialized = YamlSerializer.Deserialize<GenericAbstractImlementationLessParamsDataContract<int>>(s);
        Assert.Equal(abstractObject.Test, deserialized.Test);
    }
    [Fact()]
    public void SubstitutedGenericInheritedClass()
    {
        Setup();
        GenericImplementedClassWithLessParams<int> abstractObject = new SubstitutedGenericClassNoParams()
        {
            Generic = 3
        };
        var s = YamlSerializer.SerializeToString(abstractObject);
        var deserialized = YamlSerializer.Deserialize<GenericImplementedClassWithLessParams<int>>(s);
        Assert.Equal(abstractObject.Generic, deserialized.Generic);
    }
    [Fact()]
    public void UnregisteredRedirection()
    {
        Setup();
        UnregisteredBase abstractObject = new UnregisteredInherited()
        {
        };
        var s = YamlSerializer.SerializeToString(abstractObject);
        var deserialized = YamlSerializer.Deserialize<UnregisteredBase>(s);
        Assert.Null(deserialized);
    }
    [Fact()]
    public void Test()
    {
        var b = new ArrayBufferWriter<byte>();
        var emitter = new Utf8YamlEmitter(b);

        var x = new Homp()
        {
            Id = 1,
            Value = 2
        };
        var ser = new NexSourceGenerated_NexYamlTestHomp();
        var context = new YamlSerializationContext(new YamlSerializerOptions());
        context.IsRedirected = true;
        ser.Serialize(ref emitter, x,context);
        var result = Encoding.UTF8.GetString(b.WrittenSpan);
        var dc =new YamlDeserializationContext(new YamlSerializerOptions());
        var s = new ReadOnlySequence<byte>(b.WrittenMemory);
        var tokenizer = new Utf8YamlTokenizer(s);
        var y = new YamlParser(ref tokenizer);
        y.SkipAfter(ParseEventType.DocumentStart);
        var d = ser.Deserialize(ref y,dc);
        Assert.Equal(x.Value,d.Value);
        Assert.Equal(x.Id,d.Id);
    }
}
[DataContract]
public class Homp
{
    public int Id { get; set; }
    public int Value { get; set; }
}
[System.CodeDom.Compiler.GeneratedCode("NexVYaml", "1.0.0.0")]
file sealed class NexSourceGenerated_NexYamlTestHompHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.RegisterTag($"NexYamlTest.Homp,NexYamlTest", typeof(NexYamlTest.Homp));
        resolver.Register(this, typeof(NexYamlTest.Homp), typeof(NexYamlTest.Homp));
        var formatter = new NexYamlTest.NexSourceGenerated_NexYamlTestHomp();
        resolver.RegisterFormatter(formatter);



    }
    public IYamlFormatter Create(Type type)
    {

        return new NexSourceGenerated_NexYamlTestHomp();
    }
}
[System.CodeDom.Compiler.GeneratedCode("NexVYaml", "1.0.0.0")]
file struct NexSourceGenerated_NexYamlTestHomp : IYamlFormatter<NexYamlTest.Homp>
{

    private static readonly byte[] UTF8Id = new byte[] { 73, 100 };
    private static readonly byte[] UTF8Value = new byte[] { 86, 97, 108, 117, 101 };


    public void IndirectSerialize(ref Utf8YamlEmitter emitter, object value, YamlSerializationContext context)
    {
        Serialize(ref emitter, (NexYamlTest.Homp)value, context);
    }
    public object? IndirectDeserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        return Deserialize(ref parser, context);
    }
    public void Serialize(ref Utf8YamlEmitter emitter, NexYamlTest.Homp value, YamlSerializationContext context)
    {

        if (value is null)
        {
            emitter.WriteNull();
            return;
        }

        emitter.BeginMapping(DataStyle.Compact);
        emitter.WriteString("Id", NexVYaml.Emitter.ScalarStyle.Plain);
        context.Serialize(ref emitter, value.Id);
        emitter.WriteString("Value", NexVYaml.Emitter.ScalarStyle.Plain);
        context.Serialize(ref emitter, value.Value);

        emitter.EndMapping();
    }

    public NexYamlTest.Homp? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }
        parser.ReadWithVerify(ParseEventType.MappingStart);
        var __TEMP__Id = default(int);
        var __TEMP__Value = default(int);

        while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
        {
            if (parser.CurrentEventType != ParseEventType.Scalar)
            {
                throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
            }

            if (!parser.TryGetScalarAsSpan(out var key))
            {
                throw new YamlSerializerException(parser.CurrentMark, "Custom type deserialization supports only string key");
            }

            switch (key.Length)
            {
                case 2:
                    if (key.SequenceEqual(UTF8Id))
                    {
                        parser.Read();
                        __TEMP__Id = context.DeserializeWithAlias<int>(ref parser);
                    }
                    else
                    {
                        parser.Read();
                        parser.SkipCurrentNode();
                    }
                    continue;
                case 5:
                    if (key.SequenceEqual(UTF8Value))
                    {
                        parser.Read();
                        __TEMP__Value = context.DeserializeWithAlias<int>(ref parser);
                    }
                    else
                    {
                        parser.Read();
                        parser.SkipCurrentNode();
                    }
                    continue;

                default:
                    parser.Read();
                    parser.SkipCurrentNode();
                    continue;
            }
        }

        parser.ReadWithVerify(ParseEventType.MappingEnd);
        var __TEMP__RESULT = new NexYamlTest.Homp
        {
            Id = __TEMP__Id,
            Value = __TEMP__Value
        };

        return __TEMP__RESULT;
    }
}
