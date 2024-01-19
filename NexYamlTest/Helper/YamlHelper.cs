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
    private static void SetUp () => NexYamlSerializerRegistry.Init ();
    public static void Run<T>(T target)
        where T: IEquatable<T>
    {
        SetUp();
        var serialized = YamlSerializer.SerializeToString(target);
        Assert.Null(serialized);
        var deserialized = YamlSerializer.Deserialize<T>(serialized);
        Assert.Equal(target, deserialized);
    }
}
