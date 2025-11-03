using NexYaml;
using NexYaml.Collections;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.Collections;

public class EmptyTest
{
    [Fact]
    public async Task EmptyDictionary_int()
    {
        NexYamlSerializerRegistry.Init();
        var dictionary = new Dictionary<int, TempData>();
        var s = Yaml.Write(dictionary);
        var d = await TestParser.Read<Dictionary<int, TempData>>(s);
        Assert.NotNull(d);
        Assert.Empty(d);
    }
    [Fact]
    public async Task EmptyDictionary_ComplexKey()
    {
        NexYamlSerializerRegistry.Init();
        var dictionary = new Dictionary<TempData, TempData>();
        var s = Yaml.Write(dictionary);
        var d = await TestParser.Read<Dictionary<TempData, TempData>>(s);
        Assert.NotNull(d);
        Assert.Empty(d);
    }
    [Fact]
    public async Task NullCollections()
    {
        var collection = new NullDictionary();
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(collection);
        var d = await TestParser.Read<NullDictionary>(s);
        Assert.NotNull(d);
        Assert.Null(d.dict);
    }
    [Fact]
    public async Task Null_Dictionary()
    {
        Dictionary<int,int> collection = null!;
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(collection);
        var d = await TestParser.Read<Dictionary<int,int>>(s);
        Assert.Null(d);
    }
}
