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
    [GlobalSetup]
    public void Setup()
    {
        resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);

    }
    [IterationSetup]
    public void Init()
    {
        s = new StreamWriter(memoryStream = new MemoryStream(4000));
        writer = new YamlWriter(s,resolver,IResolvePlugin.plugins);

    }
    [Benchmark(Baseline = true)]
    public void YamlB()
    {
        writer.Write(values,DataStyle.Compact);
     
        s.Flush();

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