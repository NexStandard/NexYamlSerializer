using System.Text;
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
        IEnumerable<int> GetNumbers()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        var numbers = GetNumbers();

        foreach (var n in numbers) { Console.WriteLine(n); } // Prints 1,2,3
        foreach (var n in numbers) { Console.WriteLine(n); } // Prints nothing!
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