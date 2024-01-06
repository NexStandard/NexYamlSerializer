using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class ConstFieldTest
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact]
    public void ConstField()
    {
        Setup();
        var aliased = new ClassWithConstField()
        {
            Normal = 1,
        };
        var s = YamlSerializer.SerializeToString(aliased);
        var deserialized = YamlSerializer.Deserialize<ClassWithConstField>(s);
        Assert.Equal(aliased.Normal, deserialized.Normal);
    }
}
