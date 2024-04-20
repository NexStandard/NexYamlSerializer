using Newtonsoft.Json.Linq;
using NexVYaml;
using NexYamlTest.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.DataStyleTests;
public class CompactTest
{
    [Fact]
    public void Compact_Class()
    {
        var compact = new CompactClass()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        YamlHelper.Run(compact);
    }

    [Fact]
    public void Compact_RecordWithMember()
    {
        var compact = new CompactRecordWithCompactMember()
        {
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(compact);
        throw new Exception(s);
        Assert.Equal("!NexYamlTest.DataStyleTests.CompactRecordWithCompactMember,NexYamlTest\nCompactMember: !NexYamlTest.DataStyleTests.CompactRecord,NexYamlTest { X: 0, Y: 0, W: !!null }\n",s);
        YamlHelper.Run(compact);
    }    
    [Fact]
    public void Double_Compact_RecordWithMember()
    {
        var compact = new CompactCompactRecord()
        {
        };
        NexYamlSerializerRegistry.Init();
        var s = YamlSerializer.SerializeToString(compact);

        // Assert.Equal("!NexYamlTest.DataStyleTests.CompactRecordWithCompactMember,NexYamlTest\nCompactMember: !NexYamlTest.DataStyleTests.CompactRecord,NexYamlTest { X: 0, Y: 0, W: !!null }\n",s);
        YamlHelper.Run(compact);
    }

    [Fact]
    public void Compact_Struct()
    {
        var compact = new CompactStruct()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        YamlHelper.Run(compact);
    }
    [Fact]
    public void Compact_Record()
    {
        var compact = new CompactStruct()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        YamlHelper.Run(compact);
    }
    [Fact]
    public void Compact_Members()
    {
        var compact = new CompactMembers()
        {
            NonCompactClass = new() { W = "st", Y = 20 },
            X = new() {  X = 1 },
        };
        YamlHelper.Run(compact);
    }
    [Fact]
    public void Compact_Array()
    {
        YamlHelper.SetUp();
        var compact = new CompactArray()
        {
            Ints = [8, 7, 5]
        };
        var serialized = YamlSerializer.SerializeToString(compact);
        var deserialized = YamlSerializer.Deserialize<CompactArray>(serialized);
        Assert.Equal(compact.Ints[0] , deserialized.Ints[0]);
        Assert.Equal(compact.Ints[1] , deserialized.Ints[1]);
        Assert.Equal(compact.Ints[2] , deserialized.Ints[2]);
    }
}
