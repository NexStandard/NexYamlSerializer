using NexVYaml;
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
            Value = new int[] { 1, 2 }
        };
        var s = YamlSerializer.SerializeToString(array);
        // TODO: activate
        // Generics<int[]> d = YamlSerializer.Deserialize<Generics<int[]>>(s);
    }
}
