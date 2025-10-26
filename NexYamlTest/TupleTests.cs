using System;
using System.Threading.Tasks;
using NexYaml;
using Xunit;

namespace NexYamlTest;

public class TupleTests
{
    [Fact]
    public async Task ValueTuple2()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int>(10, 10));
        var d1 = await TestParser.Read<(int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
    }
    [Fact]
    public async Task ValueTuple3()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int, int>(10, 10, 10));
        var d1 = await TestParser.Read<(int, int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
    }
    [Fact]
    public async Task ValueTuple4()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int, int, int>(10, 10, 10, 10));
        var d1 = await TestParser.Read<(int, int, int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
    }
    [Fact]
    public async Task ValueTuple5()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int, int, int, int>(10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<(int, int, int, int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
    }
    [Fact]
    public async Task ValueTuple6()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int, int, int, int, int>(10, 10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<(int, int, int, int, int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
        Assert.Equal(10, d1.Item6);
    }
    [Fact]
    public async Task ValueTuple7()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new ValueTuple<int, int, int, int, int, int, int>(10, 10, 10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<(int, int, int, int, int, int, int)>(t1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
        Assert.Equal(10, d1.Item6);
        Assert.Equal(10, d1.Item7);
    }
    [Fact]
    public async Task Tuple2()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int>(10, 10));
        var d1 = await TestParser.Read<Tuple<int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
    }
    [Fact]
    public async Task Tuple3()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int, int>(10, 10, 10));
        var d1 = await TestParser.Read<Tuple<int, int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
    }
    [Fact]
    public async Task Tuple4()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int, int, int>(10, 10, 10, 10));
        var d1 = await TestParser.Read<Tuple<int, int, int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
    }
    [Fact]
    public async Task Tuple5()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int, int, int, int>(10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<Tuple<int, int, int, int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
    }
    [Fact]
    public async Task Tuple6()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int, int, int, int, int>(10, 10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<Tuple<int, int, int, int, int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
        Assert.Equal(10, d1.Item6);
    }
    [Fact]
    public async Task Tuple7()
    {
        NexYamlSerializerRegistry.Init();
        var t1 = Yaml.Write(new Tuple<int, int, int, int, int, int, int>(10, 10, 10, 10, 10, 10, 10));
        var d1 = await TestParser.Read<Tuple<int, int, int, int, int, int, int>>(t1);
        Assert.NotNull(d1);
        Assert.Equal(10, d1.Item1);
        Assert.Equal(10, d1.Item2);
        Assert.Equal(10, d1.Item3);
        Assert.Equal(10, d1.Item4);
        Assert.Equal(10, d1.Item5);
        Assert.Equal(10, d1.Item6);
        Assert.Equal(10, d1.Item7);
    }
}
