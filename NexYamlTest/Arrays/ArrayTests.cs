﻿using NexYaml;
using NexYaml.Core;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest.ArrayTests;
public class ArrayTests
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact]
    public void Generic_Int_Array()
    {
        Setup();
        var intArray = new Generics<int[]>()
        {
            Value = [1, 2]
        };
        var intArrayString = Yaml.Write(intArray);
        var intArrayDeserialized = Yaml.Read<Generics<int[]>>(intArrayString);
        Assert.NotNull(intArrayDeserialized);
        Assert.NotNull(intArrayDeserialized.Value);
        Assert.Equal(1, intArrayDeserialized.Value[0]);
        Assert.Equal(2, intArrayDeserialized.Value[1]);
    }
    [Fact]
    public void Generic_String_Array()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["bob0", "bob1"]
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = Yaml.Read<Generics<string[]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal("bob0", stringArrayDeserialized.Value[0]);
        Assert.Equal("bob1", stringArrayDeserialized.Value[1]);
    }
    [Fact]
    public void Generic_Nested_Int_Array()
    {
        Setup();
        var array = new Generics<int[][]>()
        {
            Value = [ [ 2 ], [ 1 ]]
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = Yaml.Read<Generics<int[][]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal([ 2 ], stringArrayDeserialized.Value[0]);
        Assert.Equal([ 1 ], stringArrayDeserialized.Value[1]);
    }
    [Fact]
    public void Pass_On_Wrong_Generic_Type_but_Equal_Output()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["1", "2"]
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = Yaml.Read<Generics<int[]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal(1, stringArrayDeserialized.Value[0]);
        Assert.Equal(2, stringArrayDeserialized.Value[1]);
    }
    [Fact]
    public void Failure_On_Wrong_Generic_Type()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["1c", "2c"]
        };
        var stringArrayString = Yaml.Write(array);
        Assert.Throws<YamlException>(() => Yaml.Read<Generics<int[]>>(stringArrayString));
    }
}
