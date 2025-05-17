
using BenchmarkDotNet.Running;
using NexYaml;
using Stride.Core;
using System.Text.Json.Serialization;
using Test;
IList<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>> list = new List<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>()
        {
            new List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>(),
            new List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>(),
        };
NexYamlSerializerRegistry.Init();
var s = Yaml.Write(list, DataStyle.Compact);
Console.WriteLine(s);
var d = Yaml.Read<IList<List<GenericAbstractLessParams<GenericAbstractLessParams<int>>>>>(s);

[DataContract]
public partial class Data
{
    [JsonInclude]
    public int Id;
    [JsonInclude]
    public string Value;
}
[DataContract]
public partial class Wrapper
{
    [JsonInclude]
    public Data[] Data;
}
