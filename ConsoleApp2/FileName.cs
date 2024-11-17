﻿using BenchmarkDotNet.Attributes;
using NexYaml;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Test;
[MemoryDiagnoser]
public class Benchmarker
{
    internal static Collections c = new Collections();
    public IYamlWriter writer;
    public UTF8Stream stream;
    static bool  x = false;
    [IterationSetup()]
    public void Setup()
    {
        if(!x)
        {
        NexYamlSerializerRegistry.Init();
            x = true;
        }
        stream = new UTF8Stream();
        writer = new YamlWriter(stream, NexYamlSerializerRegistry.Instance);
    }
    [Benchmark()]
    public void Yaml()
    {
        writer.Write(c, DataStyle.Compact);
        var s = stream.GetChars().Span.ToString();
    }
    [Benchmark()]
    public void Json()
    {
        var json = JsonSerializer.Serialize(c, MyJsonContext.Default.Collections);
    }
}
[DataContract]
internal class Collections
{
    [JsonInclude]
    public List<TempData> values;

    public Collections()
    {
        values = new List<TempData>
        {
            new TempData { Name = "John", Id = 1 },
            new TempData { Name = "Alice", Id = 2 },
            new TempData { Name = "Bob", Id = 3 },
            // Add more test data as needed
        };
    }
}

[JsonSerializable(typeof(Collections))]
[JsonSerializable(typeof(TempData))]
internal partial class MyJsonContext : JsonSerializerContext
{
}

[DataContract]
public record TempData
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Id { get; set; }
}