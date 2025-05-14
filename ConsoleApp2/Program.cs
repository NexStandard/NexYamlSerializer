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

        var context = new Context<Mapping>(-2, false, null, DataStyle.Normal, new BlockMapping());
        new BlockMapping()
            .BeginMapping(context, "!Temp", DataStyle.Normal)
            .Write("hi", new TestS())
            .Write("hi2", new TestS())
            .Write("hi4", new TestS())
            .Write("hi3", new TestS());
    }
}