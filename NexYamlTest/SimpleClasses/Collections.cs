using Stride.Core;
using System.Collections.Generic;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Collections
{
    public Dictionary<string, TempData> keyValuePairs = [];
    public Dictionary<TempData, TempData> ComplexDictionary = [];
    public Dictionary<int, int> Homp = [];
    public List<string> strings = [];
    public List<TempData> values = [];
}
[DataContract]
public record TempData
{
    public string name { get; set; }
    public int id { get; set; }
}
