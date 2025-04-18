using NexYaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.DataMemberContentMode;
public class ContentModeTest
{
    [Fact]
    public void DataMemberContentModeNotIgnored()
    {
        NexYamlSerializerRegistry.Init();
        var x = new ContentModeClass();
        var s = Yaml.Write(x);
        var d = Yaml.Read<ContentModeClass>(s);
        Assert.NotNull(d);
        Assert.Equal(12, d.Content.Generics);

    }
}
