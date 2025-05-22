using System.Threading.Tasks;
using NexYaml;
using Xunit;

namespace NexYamlTest.DataMemberContentMode;
public class ContentModeTest
{
#if NET9_0
    [Fact]
    public async Task DataMemberContentModeNotIgnored()
    {
        NexYamlSerializerRegistry.Init();
        var x = new ContentModeClass()
        {
            ContentInitRequired = new ContentModeData(14)
        };
        var s = Yaml.Write(x);
        var d = await Yaml.Read<ContentModeClass>(s);
        Assert.NotNull(d);
        Assert.Equal(12, d.Content.Generics);
        Assert.Equal(12, d.ContentInit.Generics);
        Assert.Equal(14, d.ContentInitRequired.Generics);
    }
#endif
}
