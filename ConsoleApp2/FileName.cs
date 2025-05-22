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
    static IYamlSerializerResolver? resolver = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
    static Collections values = new Collections();

    static Benchmarker()
    {
        Yaml.Write(values, (ReadOnlySpan<char> text) => { }, DataStyle.Compact, resolver);
    }
    [Benchmark]
    public void YamlB()
    {
        Yaml.Write(values, (ReadOnlySpan<char> text) => { }, DataStyle.Compact, resolver);
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
    public List<TempData> tempDatas = [
        new(){
            Id = Guid.NewGuid(),
            Name = "Name",
        },
        new(){
            Id = Guid.NewGuid(),
            Name = "Name",
        },
        new(){
            Id = Guid.NewGuid(),
            Name = "Name",
        },new(){
            Id = Guid.NewGuid(),
            Name = "Name",
        },
        new(){
            Id = Guid.NewGuid(),
            Name = "Name",
        }

        ];
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