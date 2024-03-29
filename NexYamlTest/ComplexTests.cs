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
    public void GenericNullableTest()
    {
        Setup();
        GenericAbstractImplementation<int?, int?> H = new GenericAbstractImplementation<int?, int?>
        {
            Test = 1,
            TI = 2,
            TI2 = null
        };
        var s = YamlSerializer.SerializeToString(H);
        var deserialized = YamlSerializer.Deserialize<UnregisteredBase>(s);
        Assert.Null(deserialized);
    }
}