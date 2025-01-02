using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class GenericArrayTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact]
    public void GenericArray()
    {
        Setup();
        var array = new Generics<int[]>()
        {
            Value = [1, 2]
        };
        var s = Yaml.Write(array);
        // TODO: activate
        Generics<int[]> d = Yaml.Read<Generics<int[]>>(s);
    }
}
