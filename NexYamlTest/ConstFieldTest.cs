using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class ConstFieldTest
{
    private static void Setup()
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
        var s = Yaml.WriteToString(aliased);
        var deserialized = Yaml.Read<ClassWithConstField>(s);
        Assert.Equal(aliased.Normal, deserialized.Normal);
    }
}
