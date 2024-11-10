using NexVYaml;
using System;
using Xunit;

namespace NexYamlTest.Helper;
internal class YamlHelper
{
    public static void SetUp()
    {
        NexYamlSerializerRegistry.Init();
    }

    public static void Run<T>(T target)
        where T : IEquatable<T>
    {
        SetUp();
        var serialized = YamlSerializer.SerializeToString(target);
        var deserialized = YamlSerializer.Deserialize<T>(serialized);
        Assert.Equal(target, deserialized);
    }
}
