using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class InheritanceTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }

    [Fact]
    public void Inheritance_NormalClass_NoOverride()
    {
        Setup();
        var inherited = new InheritedNormalClass()
        {
            Test = 100,
            Name = "Bob"
        };

        var s = Yaml.Write(inherited);
        var deserialized = Yaml.Read<InheritedNormalClass>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(100, deserialized.Test);
        Assert.Equal("Bob", deserialized.Name);
    }
}
