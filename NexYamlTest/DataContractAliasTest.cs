using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class DataContractAliasTest
{
    void Setup () => NexYamlSerializerRegistry.Init();
    [Fact]
    public void CheckAliasString()
    {
        Setup();
        var aliased = new DataContractAlias();
        var s = YamlSerializer.SerializeToString(aliased);
        Assert.StartsWith("!Alias",s);

    }
    [Fact]
    public void DeserializeWithAlias()
    {
        Setup();
        var aliased = new DataContractAlias()
        {
            Id = 1,
        };
        var s = YamlSerializer.SerializeToString(aliased);
        var deserialized = YamlSerializer.Deserialize<DataContractAlias>(s);
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
        var s = YamlSerializer.SerializeToString(aliased);
        var deserialized = YamlSerializer.Deserialize<IDInterface>(s);
        Assert.Equal(aliased.Id, deserialized.Id);
    }
}
