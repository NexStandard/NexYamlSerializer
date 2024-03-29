using NexVYaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.Helper;
internal class YamlHelper
{
    public static void SetUp () => NexYamlSerializerRegistry.Init ();
    public static void Run<T>(T target)
        where T: IEquatable<T>
    {
        SetUp();
        NewSerializerRegistry.Init();
        var serialized = YamlSerializer.SerializeToString(target);
        var deserialized = YamlSerializer.Deserialize<T>(serialized);
        Assert.Equal(target, deserialized);
    }
}
