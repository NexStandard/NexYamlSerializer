using NexYaml;
using System;
using System.Threading.Tasks;
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
        var deserialized = await Yaml.ReadAsync<T>(serialized);
        Assert.Equal(target, deserialized);
    }
}
