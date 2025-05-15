
using BenchmarkDotNet.Running;
using Stride.Core;
using System.Text.Json.Serialization;
using Test;

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


class Program
{

    static void Main()
    {
        BenchmarkRunner.Run<Benchmarker>();
    }
}