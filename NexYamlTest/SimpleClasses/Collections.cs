using System.Collections.Generic;
using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Collections
{
    public Dictionary<string, TempData> keyValuePairs = [];
    public Dictionary<TempData, TempData> ComplexDictionary = new Dictionary<TempData, TempData>();
    public Dictionary<int, int> Homp = [];
    public List<string> strings = [];
    public List<TempData> values = [];
    public IEnumerable<TempData> Enumerable = new List<TempData>()
    {
        new TempData(),
        new TempData(),
    };
}
[DataContract]
internal class ComplexDictionary
{
    public Dictionary<TempData, TempData> Dictionary = new();
}
[DataContract]
public record TempData
{
    public string Name { get; set; } = "";
    public int Id { get; set; }
}
