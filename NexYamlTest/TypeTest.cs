using NexYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class TypeTest
{
    [Fact]
    public void TypeDictionary()
    {
        NexYamlSerializerRegistry.Init();
        var x = new TypeDictionary();
        var s = Yaml.Write(x);
        var d = Yaml.Read<TypeDictionary>(s);
        Assert.NotNull(d);
        Assert.Equal(1, x.TypeMap[typeof(int)]);
        Assert.Equal(0, x.TypeMap[typeof(string)]);
        Assert.NotEqual(0, x.TypeMap[typeof(double)]);
    }
}
