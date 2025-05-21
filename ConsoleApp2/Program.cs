
using System.Text.Json.Serialization;
using BenchmarkDotNet.Running;
using Stride.Core;
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
