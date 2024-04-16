using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class GenericArrayTest
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact]
    public void GenericArray()
    {
        Setup();
        Generics<int[]> array = new Generics<int[]>()
        {
            Value = new int[] { 1, 2 }
        };
        var s = YamlSerializer.SerializeToString(array);
        // TODO: activate
        // Generics<int[]> d = YamlSerializer.Deserialize<Generics<int[]>>(s);
    }
}
