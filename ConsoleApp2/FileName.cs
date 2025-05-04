using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NexYaml;
using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serializers;
using SharpFont;
using Stride.Core;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Test;
[MemoryDiagnoser()]
public class Benchmarker
{
    IYamlSerializerResolver resolver;
    Collections values = new Collections();
    MemoryStream memoryStream;
    YamlWriter writer;
    StreamWriter s;
    (WriteContext,StreamWriter) context;
    [GlobalSetup]
    public void Setup()
    {
        resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
    }
    [IterationSetup]
    public void Init()
    {
        context = Yaml.GetContext();
    }
    [Benchmark(Baseline = true)]
    public void YamlB()
    {
        context.Item1.Write(values, DataStyle.Compact);
        context.Item2.Flush();
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
        Id = 2,
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
public struct TempData
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public int Id { get; set; }
}