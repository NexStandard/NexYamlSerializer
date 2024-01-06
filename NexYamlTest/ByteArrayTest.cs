using NexVYaml;
using NexYamlTest.SimpleClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class ByteArrayTest
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact]
    public void ByteArray()
    {
        Setup();
        var byteArray = new ByteArray()
        {
            Data = [1, 2]
        };
        var s = YamlSerializer.Serialize(byteArray);
        var d = YamlSerializer.Deserialize<ByteArray>(s);
        Assert.Equal(byteArray.Data, d.Data);
    }
}
