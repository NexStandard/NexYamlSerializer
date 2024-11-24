﻿using NexYaml;
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

        var s = Yaml.WriteToString(list);
        var deserialized = Yaml.Read<DoubleInheritedList>(s);
        Assert.Null(s);
        Assert.Equal(list.Count, deserialized!.Count);
        for (var i = 0; i < list.Count; i++)
        {
            Assert.Equal(list[i], deserialized[i]);
        }
    }
    [Fact]
    public void Delegates()
    {
        Setup();
        var g = new Delegates();
        var x = Yaml.WriteToString(g, Stride.Core.DataStyle.Compact);
        var t = Yaml.Read<Delegates>(x);
        throw new Exception(t.Action.ToString());
        throw new Exception(x);
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
        var s = Yaml.WriteToString(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
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
        var s = Yaml.WriteToString(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void NestedDynamicGenerics()
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
        var s = Yaml.WriteToString(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<IGenericInterface<int, int>, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic.Generic, deserialized.Generic.Generic);
        Assert.Equal(genericInterface.Generic.Generic2, deserialized.Generic.Generic2);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void NoParamsImplementation()
    {
        Setup();
        IGenericInterface<int, int> genericInterface = new GenericImplementedClassWithNoParams()
        {
            Generic2 = 1,
            Generic = 10
        };
        var s = Yaml.WriteToString(genericInterface);
        var deserialized = Yaml.Read<IGenericInterface<int, int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(genericInterface.Generic, deserialized.Generic);
        Assert.Equal(genericInterface.Generic2, deserialized.Generic2);
    }
    [Fact]
    public void InheritedSameGenerics()
    {
        Setup();
        GenericAbstract<int, int> genericInterface = new GenericAbstractImplementation<int, int>()
        {
            TI = 1,
            TI2 = 10
        };
        var s = Yaml.WriteToString(genericInterface);
        var deserialized = Yaml.Read<GenericAbstractImplementation<int, int>>(s);
        Assert.NotNull(deserialized);
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
        var s = Yaml.WriteToString(abstractObject);
        var deserialized = Yaml.Read<GenericAbstractImlementationLessParams<int>>(s);
        Assert.NotNull(deserialized);
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
        var s = Yaml.WriteToString(abstractObject);
        var deserialized = Yaml.Read<GenericAbstractImlementationLessParamsDataContract<int>>(s);
        Assert.NotNull(deserialized);
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
        var s = Yaml.WriteToString(abstractObject);
        var deserialized = Yaml.Read<GenericImplementedClassWithLessParams<int>>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(abstractObject.Generic, deserialized.Generic);
    }
    [Fact()]
    public void UnregisteredRedirection()
    {
        Setup();
        UnregisteredBase abstractObject = new UnregisteredInherited()
        {
        };
        var s = Yaml.WriteToString(abstractObject);
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
        var s = Yaml.WriteToString(H);
        var deserialized = Yaml.Read<UnregisteredBase>(s);
        Assert.Null(deserialized);
    }
}