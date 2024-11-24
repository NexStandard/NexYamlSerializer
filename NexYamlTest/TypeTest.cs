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
        var s = Yaml.WriteToString(x);
        var d = Yaml.Read<TypeDictionary>(s);
        Assert.NotNull(d);
        Assert.Equal(x.TypeMap[typeof(int)], 1);
        Assert.Equal(x.TypeMap[typeof(string)], 0);
        Assert.NotEqual(x.TypeMap[typeof(double)], 0);
    }
}
