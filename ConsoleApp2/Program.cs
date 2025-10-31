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
        BenchmarkRunner.Run<Benchmarker>();
    }
}
public record Person(int Id, string Name, bool Female);

public class PersonSerializer : IYamlSerializer<Person>
{

    public void Write<X>(NexYaml.Serialization.WriteContext<X> context, Person value, DataStyle style) where X : NexYaml.Serialization.Node
    {
        throw new NotImplementedException();
    }
    public ValueTask<Person> Read(Scope scope, Person? parseResult)
    {
        var mapping = scope.As<SequenceScope>();
        int id = default;
        string? name = default;
        bool female = default;
        foreach (var kvp in mapping)
        {

        }

        return new(new Person(id, name, female));
    }
}