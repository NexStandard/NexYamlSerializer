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
using ConsoleApp2;
using Silk.NET.OpenXR;
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
        var context = new Context<Mapping>() { Node = new FlowMapping() };
        new FlowSequence().BeginSequence(context, "!Sequence", DataStyle.Compact)
            .Write("Hi")
            .Write(new TestS())
            .Write("Hompty")
            .Write("Help")
            .End(context);
        // Yaml.Write(new Collections() { });
        // BenchmarkRunner.Run<Benchmarker>();
    }
}