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
        var s = Yaml.WriteToString(aliased);
        Assert.StartsWith("!Alias", s);

    }
    [Fact]
    public void DeserializeWithAlias()
    {
        Setup();
        var aliased = new DataContractAlias()
        {
            Id = 1,
        };
        var s = Yaml.WriteToString(aliased);
        var deserialized = Yaml.Read<DataContractAlias>(s);
        Assert.Equal(aliased.Id, deserialized.Id);
    }
    [Fact]
    public void DeserializeWithAliasOnInterface()
    {
        Setup();
        IDInterface aliased = new DataContractAlias()
        {
            Id = 1,
        };
        var s = Yaml.WriteToString(aliased);
        var deserialized = Yaml.Read<IDInterface>(s);
        Assert.Equal(aliased.Id, deserialized.Id);
    }
}
