using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BenchmarkDotNet.Attributes;
using NexYaml;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
namespace Test;
[MemoryDiagnoser()]
public class Benchmarker
{
    static Collections values;

    static IYamlSerializerResolver? resolver;
    static Benchmarker()
    {
    }
    [GlobalSetup]
    public void Gsetup()
    {
        resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
        values = new Collections();
    }
    [GlobalCleanup]
    public void Setup()
    {
        resolver = null;
        values = null;
    }
    [Benchmark]
    public async Task YamlB()
    {
       var  w = Yaml.Write(values, DataStyle.Normal, resolver);
        var parser = new YamlParser(w, resolver).Parse();
        parser.First().EmptyDump();
    }
    [Benchmark()]
    public void JsonB()
    {

        var w = JsonSerializer.Serialize(values, MyJsonContext.Default.Collections);
        var d = JsonSerializer.Deserialize<Collections>(w);
    }

}
[DataContract]
internal class Collections
{

    [JsonInclude]
    public TempData valu11es = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };
    [JsonInclude]
    public TempData valu1es = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };
    [JsonInclude]
    public TempData value3s = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };
    [JsonInclude]
    public TempData valu2es = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };
    [JsonInclude]
    public TempData value1s = new TempData
    {
        Id = Guid.NewGuid(),
        Name = $"Name_1",
    };
    [JsonInclude]
    public TempData values3 = new TempData
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