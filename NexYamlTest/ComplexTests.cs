﻿using NexVYaml;
using NexVYaml.Serialization;
using NexYamlTest.ComplexCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest;
public class ComplexTests
{
    private void Setup()
    {
        NexYamlSerializerRegistry.Init();
    }
    [Fact(Skip = "Unimplemented ICollection handling")]
    public void DoubleInheritedListTest()
    {
        Setup();
        var list = new DoubleInheritedList()
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13,
        };

        var s = YamlSerializer.SerializeToString(list);
        var deserialized = YamlSerializer.Deserialize<DoubleInheritedList>(s);
        Assert.Null(s);
        Assert.Equal(list.Count,deserialized.Count);
        for ( var i = 0; i < list.Count; i++ )
        {
            Assert.Equal(list[i], deserialized[i] );
        }
    }
    [Fact]
    public void SecureModeTest()
    {
        Setup();
        var secureObject = new SecureMode();
        IInSecure inSecure = secureObject;
        var secureMode = new YamlSerializerOptions()
        {
            Resolver = NexYamlSerializerRegistry.Instance,
            SecureMode = true
        };
        var s = YamlSerializer.SerializeToString(secureObject,secureMode);
        var deserialized = YamlSerializer.Deserialize<SecureMode>(s);

        Assert.NotNull(deserialized);
        var insecureSerialize = YamlSerializer.SerializeToString(inSecure, secureMode);
        Assert.Equal("!!null",insecureSerialize.ToString());
        var insecureDeserialize = YamlSerializer.Deserialize<IInSecure>(s, secureMode);
        Assert.Null(insecureDeserialize);
    }
}