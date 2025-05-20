
using BenchmarkDotNet.Running;
using NexYaml;
using Stride.Core;
using System.Text.Json.Serialization;
using Test;

BenchmarkRunner.Run<Benchmarker>();

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
