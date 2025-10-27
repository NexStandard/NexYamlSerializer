using System;
using System.Threading.Tasks;
using NexYaml;
using Xunit;

namespace NexYamlTest.Helper;
internal class YamlHelper
{
    public static void SetUp()
    {
        NexYamlSerializerRegistry.Init();
    }

    public static async Task Run<T>(T target)
        where T : IEquatable<T>
    {
        SetUp();
        var serialized = Yaml.Write(target);
        var d = await TestParser.Read<T>(serialized);
        Assert.Equal(target, d);
    }
}
