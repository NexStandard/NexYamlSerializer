using NexYaml;
using NexYaml.Serialization;
using NexYamlTest.ComplexCases;
using NexYamlTest.DataStyleTests;
using NexYamlTest.SimpleClasses;
using SharpFont;
using Stride.Core;
using Stride.Core.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;

public class CollectionTest
{
    [DataContract]
    public class NullDictionary
    {
        public Dictionary<int, int>? dict = null;
    }

    [Fact]
    public async Task NullCollections()
    {
        var collection = new NullDictionary();
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(collection);
        
        var d = await Yaml.Read<NullDictionary>(s);
        Assert.NotNull(d);
        Assert.Null(d.dict);
    }

    [Fact]
    public async Task EmitMixedCollection()
    {
        IList<GenericAbstract<int,int>> list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParams<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list);
        var d = await Yaml.Read<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParams<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact]
    public async Task CompactCollection_InCOllection()
    {
        IList<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>> list = new List<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>()
        {
            new List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>()
            {
                new()
            },
            new List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>()
            {
                new()
            },
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Compact);
        var d = await Yaml.Read<IList<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>>(s);
        Assert.NotNull(d);
        Assert.IsType<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>(d[0]);
        Assert.IsType< List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>(d[1]);

        var s2 = Yaml.Write(list, DataStyle.Normal);
        var d2 = await Yaml.Read<IList<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>>(s2);
        Assert.NotNull(d2);
        Assert.IsType<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>(d2[0]);
        Assert.IsType<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>(d2[1]);
    }
    [Fact]
    public async Task EmitMixedCollectionCompact()
    {
        var list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParams<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Compact);
        var d = await Yaml.Read<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParams<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact]
    public async Task EmitDoubleCompactList()
    {
        IList<IList<int>> list = 
        [
            [1],
            [2]
        ];
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Compact);
        var d = await Yaml.Read<IList<IList<int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<List<int>>(d[0]);
        Assert.IsType<List<int>>(d[1]);
        Assert.Equal(1, d[0][0]);
        Assert.Equal(2, d[1][0]);
    }
    [Fact]
    public async Task EmitMixedCollectionCompacWithEmptyType()
    {
        var list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParamsEmpty<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Compact);
        var d = await Yaml.Read<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParamsEmpty<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact]
    public async Task EmitBlockSequenceWithCompactMapping()
    {
        var list = new List<CompactStruct>
        {
            new(),
            new()
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Any);
        var d = await Yaml.Read<List<CompactStruct>>(s);
        Assert.NotNull(d);
        Assert.Contains("- { ",s);
        Assert.Equal(default, d[0]);
        Assert.Equal(default, d[1]);
    }

    [Fact]
    public async Task EmitBlockSequenceWithCompactSequence()
    {
        var list = new List<ValueTuple<int,int>>
        {
            new(10, 10),
            new(11, 11)
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list, DataStyle.Any);
        var d = await Yaml.Read<List<ValueTuple<int, int>>>(s);
        Assert.NotNull(d);
        Assert.Contains("- [ ", s);
        Assert.Equal(new(10,10), d[0]);
        Assert.Equal(new(11,11), d[1]);
    }
    [Fact]
    public async Task ComplexDictionary()
    {
        NexYamlSerializerRegistry.Init();
        var data = new ComplexDictionary();
        data.Dictionary.Add(new TempData(), new TempData());
        data.Dictionary.Add(new TempData() { Id = 2, Name = "2"}, new TempData());

        var s = Yaml.Write(data);
        var d = await Yaml.Read<ComplexDictionary>(s);
    }
    [DataContract]
    internal class C
    {
        public IEnumerable<int>? Foo { get; set; }
    }
    [Fact(Skip ="Collection initializer? it doesnt redirect to ienumerable")]
    public void EnumerableTest()
    {
        C c = new()
        {
            Foo = [1, 2, 3]
        };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(c);
        throw new System.Exception(s);
    }
    [Fact]
    public async Task ListWithNullValues()
    {
        var list = new List<IIdentifiable?>() { null, null };
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(list,DataStyle.Compact);
        var d = await Yaml.Read<List<IIdentifiable?>>(s);
        Assert.NotNull(d);
        Assert.Equal(2, list.Count);
    }
    [Fact]
    public async Task InterfaceList()
    {
        // Creating test data
        var data1 = new CollectionInterfaces()
        {
            Collection = new List<IDInterface>() { },
            ReadonlyList = [new Data1() { Id = 1 }, new Data2() { Id = 2 }],
            Dictionary = new Dictionary<int, IDInterface>() { [1] = new Data1() },
            Enumerable =
            [
                new Data1() { Id = 1 },
            ],
            ReadonlyDictioanry = new Dictionary<IDInterface, IDInterface>() { }
        };

        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(data1);
        var d = await Yaml.Read<CollectionInterfaces>(s);
        Assert.NotNull(d);
        Assert.Equal(data1.Collection.Count, d.Collection.Count);
    }
}
[DataContract]
internal class CollectionInterfaces
{
    public ICollection<IDInterface> Collection = [];
    public IReadOnlyCollection<IDInterface> ReadOnlyCollection = new List<IDInterface>();
    public IReadOnlyList<IDInterface> ReadonlyList = [];
    public IList<IDInterface> List = [];
    public IEnumerable<IDInterface> Enumerable = [];
    public IDictionary<int, IDInterface> Dictionary = new Dictionary<int, IDInterface>();
    public IReadOnlyDictionary<IDInterface, IDInterface> ReadonlyDictioanry = new Dictionary<IDInterface, IDInterface>();
}
[DataContract]
internal class Data1 : IDInterface
{
    public int Id { get; set; }
}
[DataContract]
internal class Data2 : IDInterface
{
    public int Id { get; set; }
}
