using System.Collections.Generic;
using System.Threading.Tasks;
using NexYaml;
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
    public async Task Generic_Int_Array()
    {
        Setup();
        var intArray = new Generics<int[]>()
        {
            Value = [1, 2]
        };
        var intArrayString = Yaml.Write(intArray);
        var intArrayDeserialized = await Yaml.Read<Generics<int[]>>(intArrayString);
        Assert.NotNull(intArrayDeserialized);
        Assert.NotNull(intArrayDeserialized.Value);
        Assert.Equal(1, intArrayDeserialized.Value[0]);
        Assert.Equal(2, intArrayDeserialized.Value[1]);
    }

    [Fact]
    public async Task Generic_String_Array()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["bob0", "bob1"]
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = await Yaml.Read<Generics<string[]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal("bob0", stringArrayDeserialized.Value[0]);
        Assert.Equal("bob1", stringArrayDeserialized.Value[1]);
    }

    [Fact]
    public async Task Generic_Nested_Int_Array_Normal()
    {
        Setup();
        var array = new Generics<int[][]>()
        {
            Value = [[2], [1]]
        };
        var stringArrayString = Yaml.Write(array, Stride.Core.DataStyle.Normal);
        var stringArrayDeserialized = await Yaml.Read<Generics<int[][]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal([2], stringArrayDeserialized.Value[0]);
        Assert.Equal([1], stringArrayDeserialized.Value[1]);
    }

    [Fact]
    public async Task Generic_Nested_Int_Array_Compact()
    {
        Setup();
        var array = new Generics<int[][]>()
        {
            Value = [[2], [1]]
        };
        var stringArrayString = Yaml.Write(array, Stride.Core.DataStyle.Compact);
        var stringArrayDeserialized = await Yaml.Read<Generics<int[][]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal([2], stringArrayDeserialized.Value[0]);
        Assert.Equal([1], stringArrayDeserialized.Value[1]);
    }

    [Fact]
    public async Task Generic_Nested_Int_List_Compact()
    {
        Setup();
        var array = new List<List<int>>()
        {
            new List<int>() { 1 },
            new List<int>() { 2 },
        };
        var stringArrayString = Yaml.Write(array, Stride.Core.DataStyle.Compact);
        var stringArrayDeserialized = await Yaml.Read<List<List<int>>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized[0]);
        Assert.NotNull(stringArrayDeserialized[1]);
        Assert.Equal(1, stringArrayDeserialized[0][0]);
        Assert.Equal(2, stringArrayDeserialized[1][0]);
    }
    [Fact]
    public async Task Pass_On_Wrong_Generic_Type_but_Equal_Output()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["1", "2"]
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = await Yaml.Read<Generics<int[]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Equal(1, stringArrayDeserialized.Value[0]);
        Assert.Equal(2, stringArrayDeserialized.Value[1]);
    }
    [Fact]
    public async Task Write_Empty_Array()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = []
        };
        var stringArrayString = Yaml.Write(array);
        var stringArrayDeserialized = await Yaml.Read<Generics<int[]>>(stringArrayString);
        Assert.NotNull(stringArrayDeserialized);
        Assert.NotNull(stringArrayDeserialized.Value);
        Assert.Empty(stringArrayDeserialized.Value);
    }
    [Fact]
    public async Task Failure_On_Wrong_Generic_Type()
    {
        Setup();
        var array = new Generics<string[]>()
        {
            Value = ["1c", "2c"]
        };
        var stringArrayString = Yaml.Write(array);
        await Assert.ThrowsAsync<YamlException>(async () => await Yaml.Read<Generics<int[]>>(stringArrayString));
    }
}
