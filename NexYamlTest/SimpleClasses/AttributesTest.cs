using NexVYaml;
using Stride.Core;
using Xunit;

namespace NexYamlTest.SimpleClasses;
public class AttributeTest
{
    private static void Setup() => NexYamlSerializerRegistry.Init();
    [Fact]
    public void BaseAttributesTest()
    {
        Setup();
        var dat = new Attribute()
        {
            InternalVisible = 100,
            X = 99,
            Z = 200,
        };
        var s = YamlSerializer.Serialize(dat);
        var d = YamlSerializer.Deserialize<Attribute>(s);
        Assert.Equal(100, d.InternalVisible);
        Assert.Equal(101, d.X);
        Assert.Equal(101, d.Z);
    }
}
[DataContract]
internal class Attribute
{
    [DataMemberIgnore]
    public int X = 101;
    [DataMember(DataMemberMode.Never)]
    public int Z = 101;
    [DataMember]
    internal int InternalVisible = 0;
}