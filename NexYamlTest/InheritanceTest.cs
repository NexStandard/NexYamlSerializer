using NexVYaml;
using NexVYaml.Serialization;
using NexYamlTest.ComplexCases;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class InheritanceTest
{
    private void Setup() => NexYamlSerializerRegistry.Init();
    [Fact]
    public void Inheritance_NormalClass_NoOverride()
    {
        Setup();
        var inherited = new InheritedNormalClass()
        {
            Test = 100,
            Name = "Bob"
        };
        
        var s = YamlSerializer.SerializeToString(inherited);
        var deserialized = YamlSerializer.Deserialize<InheritedNormalClass>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(100, deserialized.Test);
        Assert.Equal("Bob", deserialized.Name);
    }
}
