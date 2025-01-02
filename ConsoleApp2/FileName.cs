﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NexYaml;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Test;
[MemoryDiagnoser(false)]
public class Benchmarker
{
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