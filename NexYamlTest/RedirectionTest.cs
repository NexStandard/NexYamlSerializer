﻿using NexVYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class RedirectionTest
{
    void Setup() => NexYamlSerializerRegistry.Init();
    [Fact]
    public void InterfaceTester()
    {
        Setup();
        IDInterface dInterface = new Interfacing()
        {
            Id = 10235
        };
        var s = YamlSerializer.Serialize(dInterface);
        var deserialized = YamlSerializer.Deserialize<IDInterface>(s);
        Assert.Equal(dInterface.Id, deserialized.Id);
    }
    [Fact]
    public void AbstractTester()
    {
        Setup();
        IDAbstract dInterface = new Abstracting()
        {
            Id = 10235
        };
        var s = YamlSerializer.Serialize(dInterface);
        var deserialized = YamlSerializer.Deserialize<IDAbstract>(s);
        Assert.Equal(dInterface.Id, deserialized.Id);
    }
    [Fact]
    public void GenericSimpleTest()
    {
        Setup();
        var generic = new Generics<int>()
        {
            Value = 10235
        };
        var s = YamlSerializer.Serialize(generic);
        var deserialized = YamlSerializer.Deserialize<Generics<int>>(s);
        Assert.Equal(generic.Value, deserialized.Value);
    }
    [Fact]
    public void StackedGenericsStack()
    {
        Setup();
        var generic = new GenericWithRestriction<Generics<int>>()
        {
            Value = new Generics<int>() { Value = 1 }
        };
        var s = YamlSerializer.Serialize(generic);
        var deserialized = YamlSerializer.Deserialize<GenericWithRestriction<Generics<int>>>(s);
        Assert.Equal(generic.Value.Value, deserialized.Value.Value);
    }
}