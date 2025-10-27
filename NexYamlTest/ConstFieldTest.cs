using System.Threading.Tasks;
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
    public async Task ConstField()
    {
        Setup();
        var aliased = new ClassWithConstField()
        {
            Normal = 1,
        };
        var s = Yaml.Write(aliased);
        var deserialized = await TestParser.Read<ClassWithConstField>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(aliased.Normal, deserialized.Normal);
    }
}
