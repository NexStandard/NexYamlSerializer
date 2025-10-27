using System.Text;
using BenchmarkDotNet.Attributes;
using NexYaml;
using NexYaml.Parser;
using Stride.Core;

class Program
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    public static async Task Main(string[] args)
    {
        // BenchmarkRunner.Run<Benchmarker>();
        // Example YAML input

        string yaml = @"!NexYamlTest.References.ReferenceScriptList,NexYamlTest
List: 
- Reference: 
    Id: 00000000-0000-0000-0000-000000000000
    ReferenceScript: !!ref 0df63044-c6e5-4ee3-b938-c3280c8d7b07
    Test: 0
  Reference1: !!null
  Reference2: !!null
  Id: 2f2bd1ee-a1aa-49cf-9757-fb4dc2bbf211
- Reference: !!ref 00000000-0000-0000-0000-000000000000
  Reference1: !!null
  Reference2: !!null
  Id: 0df63044-c6e5-4ee3-b938-c3280c8d7b07
- Reference: !!ref 00000000-0000-0000-0000-000000000000
  Reference1: !!null
  Reference2: !!null
  Id: f9c689a9-6119-47dd-857c-9d7cb895351d
";
        var s = new YamlParser(yaml, new NexYamlSerializerRegistry()).Parse();
        var first = s.First();

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
[MemoryDiagnoser]
public class Benchmarker
{
    [Benchmark]
    public void Test()
    {

    }
}