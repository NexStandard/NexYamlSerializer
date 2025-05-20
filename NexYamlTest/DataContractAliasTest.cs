using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class DataContractAliasTest
{
    private static void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }

    [Fact]
    public void CheckAliasString()
    {
        Setup();
        var aliased = new DataContractAlias();
        var s = Yaml.Write(aliased);
        Assert.StartsWith("!Alias", s);

    }
    [Fact]
    public async Task  DeserializeWithAlias()
    {
        Setup();
        var aliased = new DataContractAlias()
        {
            Id = 1,
        };
        var s = Yaml.Write(aliased);
        var deserialized = await Yaml.Read<DataContractAlias>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(aliased.Id, deserialized.Id);
    }
    [Fact]
    public async Task DeserializeWithAliasOnInterface()
    {
        Setup();
        IDInterface aliased = new DataContractAlias()
        {
            Id = 1,
        };
        var s = Yaml.Write(aliased);
        var deserialized = await Yaml.Read<IDInterface>(s);
        Assert.NotNull(deserialized);
        Assert.Equal(aliased.Id, deserialized.Id);
    }
}
