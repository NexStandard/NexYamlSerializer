using System.Threading.Tasks;
using NexYaml;
using Xunit;

namespace NexYamlTest.DataMemberContentMode;
public class ContentModeTest
{
    [Fact]
    public async Task DataMemberContentModeNotIgnored()
    {
        NexYamlSerializerRegistry.Init();
        var x = new ContentModeClass();
        var s = Yaml.Write(x);
        var d = await Yaml.Read<ContentModeClass>(s);
        Assert.NotNull(d);
        Assert.Equal(12, d.Content.Generics);
    }
}
