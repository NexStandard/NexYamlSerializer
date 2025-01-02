using NexYaml;
using NexYamlTest.SimpleClasses;
using Xunit;

namespace NexYamlTest;
public class ByteArrayTest
{
    private static void Setup()
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
        var s = Yaml.Write(byteArray);
        var d = Yaml.Read<ByteArray>(s);
        Assert.Equal(byteArray.Data, d.Data);
    }
}
