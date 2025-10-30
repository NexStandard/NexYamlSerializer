using System.Text;
using BenchmarkDotNet.Characteristics;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Running;
using NexYaml;
using NexYaml.Parser;
using Stride.Core;
using Test;

class Program
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    public static async Task Main(string[] args)
    {
        var w = Yaml.Write(new Collections(), DataStyle.Normal, NexYamlSerializerRegistry.Create(typeof(Collections).Assembly));
        var parser = new YamlParser(w, NexYamlSerializerRegistry.Create(typeof(Collections).Assembly)).Parse();
        parser.First().EmptyDump();
    }
}
public record Person(int Id, string Name, bool Female);
public class PersonSerializer : YamlSerializer<Person>
{

    public override void Write<X>(NexYaml.Serialization.WriteContext<X> context, Person value, DataStyle style)
    {
        throw new NotImplementedException();
    }
    public override async ValueTask<Person?> Read(Scope scope, NexYaml.Parser.ParseContext parseResult)
    {
        var mapping = scope.As<SequenceScope>();
        int id = default;
        string? name = default;
        bool female = default;
        foreach (var kvp in mapping)
        {

        }

        return new Person(id, name, female);
    }
}