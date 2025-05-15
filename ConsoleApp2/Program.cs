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
using System.Buffers;
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
        var w = new Writer((ReadOnlySpan<char> text) => Console.Write(text.ToString()));
        var context = new Context<Mapping>(-2, false, DataStyle.Normal, new BlockMapping(), w);
        new BlockSequence()
            .BeginSequence(context, "!F", DataStyle.Normal)
            .Write(new TestS())
            .Write(new TestS())
            .Write("Hi");


    }
}