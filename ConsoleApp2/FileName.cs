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
    static Collections values = new Collections();

    static IYamlSerializerResolver? resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
    static string s;
    static string w;
    static Benchmarker()
    {
        s = JsonSerializer.Serialize(values, MyJsonContext.Default.Collections);
       w = Yaml.Write(values, DataStyle.Normal, resolver);
    }


    [Benchmark]
    public async Task YamlB()
    {
        var parser = new YamlParser(w, resolver).Parse();
        await parser.First().Read(default(Collections));
    }
    [Benchmark()]
    public void JsonB()
    {

        var d = JsonSerializer.Deserialize<Collections>(s);
    }

}
[DataContract]
public struct X
{

}
[DataContract]
public struct Y
{
    public X x;
}
[DataContract]
internal sealed class Collections
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