using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using NexYaml;
using NexYaml.Serialization;
using Stride.Core;
namespace Test;
[MemoryDiagnoser()]
public class Benchmarker
{
    IYamlSerializerResolver? resolver;
    Collections values = new Collections();
    StringBuilder s = new();

    [GlobalSetup]
    public void Setup()
    {
        resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
    }
    [IterationSetup]
    public void Init()
    {
        s = new();
    }

    [Benchmark(Baseline = true)]
    public void YamlB()
    {
        Yaml.Write(values, (ReadOnlySpan<char> text) => s.Append(text), DataStyle.Compact, resolver);
        var x = s.ToString();
    }
    [Benchmark()]
    public void JsonB()
    {
        var x = JsonSerializer.Serialize(values, MyJsonContext.Default.Collections);
    }
}
[DataContract]
internal class Collections
{
    [JsonInclude]
    public TempData values = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };

    public Collections()
    {

    }
}

[JsonSerializable(typeof(Collections))]
[JsonSerializable(typeof(TempData))]
internal partial class MyJsonContext : JsonSerializerContext
{
}

[DataContract]
public struct TempData : IIdentifiable
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public Guid Id { get; set; }
}