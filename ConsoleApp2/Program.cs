using BenchmarkDotNet.Running;
using NexYaml.Serialization;
using NexYaml;
using Stride.Core;
using System.IO;
using System.Runtime.CompilerServices;
using Test;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Text.Json;
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
        // Yaml.Write(new Collections() { });
        BenchmarkRunner.Run<Benchmarker>();
    }
}