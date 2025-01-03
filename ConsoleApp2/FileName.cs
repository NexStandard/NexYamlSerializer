using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NexYaml;
using NexYaml.Serialization;
using NexYaml.Serializers;
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
    public List<TempData> values = new();

    public static void Collectio()
    {
        List<TempData> values = new();
        for (var i = 0; i < 100000; i++)
        {
            values.Add(new TempData { Name = "John", Id = 1 });
        }
        var reg = NexYamlSerializerRegistry.Create(typeof(Collections).Assembly);
        new ListSerializerFactory().Register(reg);
        var mem = new MemoryStream();
        Yaml.Write(values, mem, DataStyle.Any,reg);
    }
    public void Bench()
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