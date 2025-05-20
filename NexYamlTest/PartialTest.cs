using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class PartialTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }

    [Fact]
    public async Task SimplePartialTest()
    {
        Setup();
        var simplePartial1 = new SimplePartial()
        {
            Id1 = 1,
            ID2 = 2,
        };
        var s = Yaml.Write(simplePartial1);
        var deserialized = await Yaml.ReadAsync<SimplePartial>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(simplePartial1.Id1, deserialized.Id1);
        Assert.Equal(simplePartial1.ID2, deserialized.ID2);

    }
}
