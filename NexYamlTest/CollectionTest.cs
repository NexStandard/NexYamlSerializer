﻿using NexYaml;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using Stride.Core;
using System.Collections.Generic;
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
    public void NullCollections()
    {
        var collection = new NullDictionary();
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(collection);
        var d = YamlSerializer.Deserialize<NullDictionary>(s);
        Assert.NotNull(d);
        Assert.Null(d.dict);
    }

    [Fact]
    public void EmitMixedCollection()
    {
        var list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParams<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(list);
        var d = YamlSerializer.Deserialize<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParams<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact]
    public void EmitMixedCollectionCompact()
    {
        var list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParams<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(list, DataStyle.Compact);
        var d = YamlSerializer.Deserialize<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParams<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact]
    public void EmitMixedCollectionCompacWithEmptyType()
    {
        var list = new List<GenericAbstract<int, int>>
        {
            new GenericAbstractImlementationLessParamsEmpty<int>(),
            new GenericAbstractImplementation<int, int>() { TI = 1, TI2 = 3 }
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(list, DataStyle.Compact);
        var d = YamlSerializer.Deserialize<List<GenericAbstract<int, int>>>(s);
        Assert.NotNull(d);
        Assert.IsType<GenericAbstractImlementationLessParamsEmpty<int>>(d[0]);
        Assert.IsType<GenericAbstractImplementation<int, int>>(d[1]);
    }
    [Fact(Skip = "doesnt assert anything yet")]
    public void Collection()
    {
        // Creating test data
        var data1 = new TempData()
        {
            Name = "John",
            Id = 1,
        };
        var data2 = new TempData()
        {
            Name = "Alice",
            Id = 2,
        };

        // Creating an object of the Collections class
        var testCollections = new Collections();

        testCollections.Homp.Add(1, 2);
        // Filling the keyValuePairs dictionary
        testCollections.keyValuePairs.Add("Key1", data1);
        testCollections.keyValuePairs.Add("Key2", data2);

        // Filling the ComplexDictionary dictionary
        testCollections.ComplexDictionary.Add(data1, data2);
        testCollections.ComplexDictionary.Add(data2, data1);

        // Filling the strings list
        testCollections.strings.Add("String1");
        testCollections.strings.Add("String2");

        // Filling the values list
        testCollections.values.Add(data1);
        testCollections.values.Add(data2);

        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(testCollections);

        var d = YamlSerializer.Deserialize<Collections>(s);

    }
    [Fact]
    public void InterfaceList()
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
        var s = YamlSerializer.SerializeToString(data1);
        var d = YamlSerializer.Deserialize<CollectionInterfaces>(s);
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
