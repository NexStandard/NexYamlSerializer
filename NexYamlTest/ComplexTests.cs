using NexYaml;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using System;
using Xunit;
namespace NexYamlTest;
public class ComplexTests
{
    private static void Setup()
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

        var s = Yaml.Write(list);
        var deserialized = Yaml.Read<DoubleInheritedList>(s);
        Assert.Null(s);
        Assert.Equal(list.Count, deserialized!.Count);
        for (var i = 0; i < list.Count; i++)
        {
            Assert.Equal(list[i], deserialized[i]);
        }
    }
    [Fact]
    public void Delegate_Serialization_Preserves_Single_Invocation()
    {
        Setup();
        var g = new Delegates();
        var x = Yaml.Write(g, Stride.Core.DataStyle.Compact);
        var t = Yaml.Read<Delegates>(x);
        // Expect Delegate to not be empty
        Assert.NotNull(t);
        // Expect to have an Invocation in the Delegate
        Assert.Single(t.Action.GetInvocationList());
    }

    [Fact]
    public void Generic_Serialization_Same_Count_Generic_Parameters_Than_Interface()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClass<int, int>()
        {
            Generic2 = 1,
            Generic = 4
        };
        var s = Yaml.Write(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void Generic_Serialization_Less_Generic_Parameters_Than_Interface()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClassWithLessParams<int>()
        {
            Generic2 = 1,
            Generic = 4
        };
        var s = Yaml.Write(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void Generic_Serialization_Nested_Generic_Parameters()
    {
        Setup();
        IGenericInterface<IGenericInterface<int, int>, int> genericInterface = new GenericImplementedClassWithLessParams<IGenericInterface<int, int>>()
        {
            Generic2 = 1,
            Generic = new GenericImplementedClassWithLessParams<int>()
            {
                Generic2 = 2,
                Generic = 4
            }
        };
        var s = Yaml.Write(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<IGenericInterface<int, int>, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic.Generic, deserialized.Generic.Generic);
        Assert.Equal(genericInterface.Generic.Generic2, deserialized.Generic.Generic2);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void Generic_Serialization_Fixed_Generic_Parameters_Of_Interface()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClassWithNoParams()
        {
            Generic2 = 1,
            Generic = 10
        };
        var s = Yaml.Write(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact()]
    public void Generic_Serialization_Fixed_Generic_Parameters_Of_Parent_Class()
    {
        Setup();
        GenericImplementedClassWithLessParams<int> abstractObject = new SubstitutedGenericClassNoParams()
        {
            Generic = 3
        };
        var s = Yaml.Write(abstractObject);
        var deserialized = Yaml.Read<GenericImplementedClassWithLessParams<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(abstractObject.Generic, deserialized.Generic);
    }
    [Fact]
    public void Generic_Serialization_Same_Generic_Parameters_Amount_As_Parent()
    {
        Setup();
        GenericAbstract<int, int> genericInterface = new GenericAbstractImplementation<int, int>()
        {
            TI = 1,
            TI2 = 10
        };
        var s = Yaml.Write(genericInterface);
        var deserialized = Yaml.Read<GenericAbstractImplementation<int, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Test, deserialized.Test);
    }
    [Fact]
    public void InheritedNoDatacontractOnAbstractClass()
    {
        Setup();
        GenericAbstract<int, int> abstractObject = new GenericAbstractImlementationLessParams<int>()
        {
            Test = 3
        };
        var s = Yaml.Write(abstractObject);
        var deserialized = Yaml.Read<GenericAbstractImlementationLessParams<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(0, deserialized.Test);
    }
    [Fact]
    public void InheritedNoDatacontractOnAbstractClassWithDataContract()
    {
        Setup();
        GenericAbstractWithDataContract<int, int> abstractObject = new GenericAbstractLessParams<int>()
        {
            Test = 3
        };
        var s = Yaml.Write(abstractObject);
        var deserialized = Yaml.Read<GenericAbstractLessParams<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(abstractObject.Test, deserialized.Test);
    }

    [Fact()]
    public void UnregisteredRedirection()
    {
        Setup();
        UnregisteredBase abstractObject = new UnregisteredInherited()
        {
        };
        var s = Yaml.Write(abstractObject);
        var deserialized = Yaml.Read<UnregisteredBase>(s);
        Assert.Null(deserialized);
    }
    [Fact()]
    public void GenericNullableTest()
    {
        Setup();
        var H = new GenericAbstractImplementation<int?, int?>
        {
            Test = 1,
            TI = 2,
            TI2 = null
        };
        var s = Yaml.Write(H);
        var deserialized = Yaml.Read<UnregisteredBase>(s);
        Assert.Null(deserialized);
    }
}