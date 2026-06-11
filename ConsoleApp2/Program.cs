using System.Text;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Running;
using NexYaml;
using NexYaml.Parser;
using Stride.Core;
using Test;

class Program
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    public static async Task Main(string[] args)
    {
        BenchmarkRunner.Run<Benchmarker>();
    }
    }
