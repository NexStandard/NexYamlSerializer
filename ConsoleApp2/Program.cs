using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using NexYaml;
using NexYaml.Serialization;
using NexYaml.Serializers;
using NexYaml.XParser;
using Stride.Core;
using Vortice.Vulkan;

class Program
{
    static Stream ToStream(string s) => new MemoryStream(Encoding.UTF8.GetBytes(s));
    public static async Task Main(string[] args)
    {
        // Example YAML input
        string yaml = @"!List [ 1, 2, 3 ]
";
        var s = new YamlParser(yaml, IYamlSerializerResolver.Default).Parse();
        var first = s.First();
        var x = await new PersonSerializer().Read(first, new NexYaml.Parser.ParseContext());
        Console.WriteLine(x);
        Console.ReadKey();
    }
}
public record Person(int Id,string Name, bool Female);
public class PersonSerializer : YamlSerializer<Person>
{

    public override void Write<X>(NexYaml.Serialization.WriteContext<X> context, Person value, DataStyle style)
    {
        throw new NotImplementedException();
    }
    public override async ValueTask<Person?> Read(Scope scope, NexYaml.Parser.ParseContext parseResult)
    {
        var mapping = scope.As<NexYaml.XParser.SequenceScope>();
        int id = default;
        string? name = default;
        bool female = default;
        foreach (var kvp in mapping)
        {

        }

        return new Person(id, name, female);
    }
}
