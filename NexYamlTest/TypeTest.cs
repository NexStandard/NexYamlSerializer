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
        var s = YamlSerializer.SerializeToString(x);
        var d = YamlSerializer.Deserialize<TypeDictionary>(s);
        Assert.NotNull(d);
    }
}
