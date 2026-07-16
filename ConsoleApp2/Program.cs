using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Running;
using NexYaml;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;
using Test;
using Vortice.Vulkan;

class Program
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    public static async Task Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarker>();

    }
}