using NexVYaml;
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
    public void SimplePartialTest()
    {
        Setup();
        var simplePartial1 = new SimplePartial()
        {
            Id1 = 1,
            ID2 = 2,
        };
        var s = YamlSerializer.SerializeToString(simplePartial1);
        var deserialized = YamlSerializer.Deserialize<SimplePartial>(s);
        Assert.Equal(simplePartial1.Id1, deserialized.Id1);
        Assert.Equal(simplePartial1.ID2, deserialized.ID2);

    }
}
