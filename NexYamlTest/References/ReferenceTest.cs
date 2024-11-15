using NexYaml;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.References;
public class ReferenceTest
{
    [Fact]
    public void WriteReferences()
    {
        NexYamlSerializerRegistry.Init();
        Guid guid = Guid.NewGuid();
        var refData = new ReferenceData()
        {
            Id = guid,
            Test = 10
        };
        var refScript = new ReferenceScript()
        {
            Reference = refData,
            Reference1 = refData,
        };
        var s = YamlSerializer.SerializeToString(refScript);
        var d = YamlSerializer.Deserialize<ReferenceScript>(s);
        Assert.Equal(refScript.Reference, refScript.Reference1);
    }
}
