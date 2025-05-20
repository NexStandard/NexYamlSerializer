
using BenchmarkDotNet.Running;
using NexYaml;
using Stride.Core;
using System.Text.Json.Serialization;
using Test;

TempData sd = new TempData();
NexYamlSerializerRegistry.Init();
var s = Yaml.Write(sd);

var d = await Yaml.Read<TempData>(s);
Console.WriteLine(d);
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
