﻿using System.Threading.Tasks;
using NexYaml;
using NexYamlTest.Helper;
using NexYamlTest.SimpleClasses;
using Stride.Core;
using Xunit;

namespace NexYamlTest.DataStyleTests;
public class CompactTest
{
    [Fact]
    public async Task Compact_Class()
    {
        var compact = new CompactClass()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        await YamlHelper.Run(compact);
    }
    [Fact]
    public async Task EmptyCompactClass()
    {
        NexYamlSerializerRegistry.Init();
        var compact = new EmptyClass();
        var s = Yaml.Write(compact, DataStyle.Any);
        var d = await Yaml.Read<EmptyClass>(s);
        Assert.NotNull(d);
        Assert.IsType<EmptyClass>(d);
    }
    [Fact]
    public void Compact_RecordWithMember()
    {
        var compact = new CompactRecordWithCompactMember();
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(compact);

        Assert.Equal("!NexYamlTest.DataStyleTests.CompactRecordWithCompactMember,NexYamlTest { CompactMember: !NexYamlTest.DataStyleTests.CompactRecord,NexYamlTest { X: 0, Y: 0, W: !!null } }", s);
    }
    [Fact]
    public async Task Compact_List()
    {
        var compact = new CompactList();
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(compact);
        var d = await Yaml.Read<CompactList>(s);
        Assert.True(d!.Lists[0] is not null);
        Assert.True(d.Lists[1] is not null);
    }
    [Fact]
    public void Double_Compact_RecordWithMember()
    {
        var compact = new CompactCompactRecord();
        NexYamlSerializerRegistry.Init();
        var s = Yaml.Write(compact);
        Assert.Equal("!NexYamlTest.DataStyleTests.CompactCompactRecord,NexYamlTest\nCompactMember: !NexYamlTest.DataStyleTests.CompactRecordWithCompactMember,NexYamlTest { CompactMember: !NexYamlTest.DataStyleTests.CompactRecord,NexYamlTest { X: 0, Y: 0, W: !!null } }", s);
    }

    [Fact]
    public async Task Compact_Struct()
    {
        var compact = new CompactStruct()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        await YamlHelper.Run(compact);
    }
    [Fact]
    public async Task Compact_Record()
    {
        var compact = new CompactStruct()
        {
            W = "dsaf",
            X = 1,
            Y = 2,
        };
        await YamlHelper.Run(compact);
    }
    [Fact]
    public async Task Compact_Members()
    {
        var compact = new CompactMembers()
        {
            NonCompactClass = new() { W = "st", Y = 20 },
            X = new() { X = 1 },
        };
        await YamlHelper.Run(compact);
    }
    [Fact]
    public async Task Compact_Array()
    {
        YamlHelper.SetUp();
        var compact = new CompactArray()
        {
            Ints = [8, 7, 5]
        };
        var serialized = Yaml.Write(compact);
        var deserialized = await Yaml.Read<CompactArray>(serialized);
        Assert.NotNull(deserialized);
        Assert.Equal(compact.Ints[0], deserialized.Ints[0]);
        Assert.Equal(compact.Ints[1], deserialized.Ints[1]);
        Assert.Equal(compact.Ints[2], deserialized.Ints[2]);
    }
}
