using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Options;
using NexYaml;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace Test;
[MemoryDiagnoser()]
public class Benchmarker
{
    static Collections values = new Collections();

    static IYamlSerializerResolver? resolver = NexYamlSerializerRegistry.Instance;
    static Writer writer;
    static string s;
    static string w;
    static Benchmarker()
    {
        NexYamlSerializerRegistry.Init();
        writer = new Writer(resolver);
    }


    [Benchmark]
    public void YamlB()
    {

        w = Yaml.Write(values, writer, DataStyle.Normal);
    }
    [Benchmark()]
    public void JsonB()
    {

        s = JsonSerializer.Serialize(values, MyJsonContext.Default.Collections);
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
public sealed class Collections
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
public struct TempData
{
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public Guid Id { get; set; }
}