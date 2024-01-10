using NexVYaml;
using NexYamlTest.SimpleClasses;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class CollectionTest
{
    [Fact]
    public void Collection()
    {
        // Creating test data
        TempData data1 = new TempData()
        {
            name = "John",
            id = 1,
        };
        TempData data2 = new TempData()
        {
            name = "Alice",
            id = 2,
        };

        // Creating an object of the Collections class
        Collections testCollections = new Collections();

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
        // Asserts
        Assert.NotNull(d);
        Assert.NotNull(d.keyValuePairs);
        Assert.Equal(2, d.Homp[1]);
        Assert.Equal(2, d.keyValuePairs.Count);
        Assert.True(d.keyValuePairs.ContainsKey("Key1"));
        Assert.True(d.keyValuePairs.ContainsKey("Key2"));
        Assert.Equal(data1, d.keyValuePairs["Key1"]);
        Assert.Equal(data2, d.keyValuePairs["Key2"]);

        Assert.NotNull(d.ComplexDictionary);
        Assert.Equal(2, d.ComplexDictionary.Count);
        Assert.True(d.ComplexDictionary.ContainsKey(data1));
        Assert.True(d.ComplexDictionary.ContainsKey(data2));
        Assert.Equal(data2, d.ComplexDictionary[data1]);
        Assert.Equal(data1, d.ComplexDictionary[data2]);

        Assert.NotNull(d.strings);
        Assert.Equal(2, d.strings.Count);
        Assert.Contains("String1", d.strings);
        Assert.Contains("String2", d.strings);

        Assert.NotNull(d.values);
        Assert.Equal(2, d.values.Count);
        Assert.Contains(data1, d.values);
        Assert.Contains(data2, d.values);
    }
    [Fact]
    public void InterfaceList()
    {
        // Creating test data
        InterfaceList data1 = new InterfaceList()
        {
            keyValuePairs = new List<IDInterface>() { new Data1(), new Data2() }
        };

        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(data1);

        var d = YamlSerializer.Deserialize<InterfaceList>(s);
        Assert.Equal(data1.keyValuePairs.Count, d.keyValuePairs.Count);
    }
}
[DataContract]
internal class InterfaceList
{
    public ICollection<IDInterface> keyValuePairs = new List<IDInterface>();
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
