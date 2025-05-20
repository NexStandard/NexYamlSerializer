using NexYaml;
using Silk.NET.SDL;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NexYamlTest.References;
public class ReferenceTest
{
    [Fact]
    public async Task ResolveClassReferences()
    {
        NexYamlSerializerRegistry.Init();
        Guid guid = Guid.NewGuid();
        var refData = new ReferenceClass()
        {
            Id = guid,
            Test = 10
        };
        var refScript = new ReferenceScript()
        {
            Reference = refData,
            Reference1 = refData,
            Reference2 = refData
        };
        var s = Yaml.Write(refScript);
        var d = await Yaml.Read<ReferenceScript>(s);
        Assert.NotNull(d);
        Assert.Equal(d.Reference, d.Reference1);
        Assert.Equal(d.Reference, d.Reference2);
    }
    [Fact]
    public async Task ResolveRefAtLaterPoint()
    {
        NexYamlSerializerRegistry.Init();
        var s = "!NexYamlTest.References.ReferenceScript,NexYamlTest\nReference1: !!ref a2c132c6-7761-425a-9cf6-b7ced923074f\nReference: !!ref a2c132c6-7761-425a-9cf6-b7ced923074f\nReference2: \n  Id: a2c132c6-7761-425a-9cf6-b7ced923074f\r\n  ReferenceScript: !!null\n  Test: 10\n";
        var d = await Yaml.Read<ReferenceScript>(s);
        Assert.NotNull(d);
        Assert.NotNull(d.Reference);
        Assert.NotNull(d.Reference1);
        Assert.NotNull(d.Reference2);
        Assert.Equal(d.Reference, d.Reference1);
        Assert.Equal(d.Reference, d.Reference2);
        Assert.Equal(d.Reference.Id, d.Reference1.Id);
    }
    [Fact]
    public async Task ResolveRecordReferences()
    {
        NexYamlSerializerRegistry.Init();
        Guid guid = Guid.NewGuid();
        var refData = new ReferenceClass()
        {
            Id = guid,
            Test = 10
        };
        var refScript = new ReferenceRecordScript()
        {
            Reference = refData,
            Reference1 = refData,
            Reference2 = refData
        };
        var s = Yaml.Write(refScript);
        var d = await Yaml.Read<ReferenceScript>(s);
        Assert.NotNull(d);
        Assert.Equal(d.Reference, d.Reference1);
        Assert.Equal(d.Reference, d.Reference2);
    }
    [Fact]
    public async Task ResolveListReferences_SameReferences()
    {
        NexYamlSerializerRegistry.Init();
        var list = new ReferenceList();
        Guid guid = Guid.NewGuid();
        var refData = new ReferenceClass()
        {
            Id = guid,
            Test = 10
        };
        list.List.Add(refData);
        list.List.Add(refData);
        list.List.Add(refData);
        var s = Yaml.Write(list);
        var d = await Yaml.Read<ReferenceList>(s);
        Assert.NotNull(d);
        Assert.NotNull(d.List[0]);
        Assert.NotNull(d.List[1]);
        Assert.Equal(d.List[0], d.List[1]);
        Assert.Equal(d.List[0], d.List[2]);
    }
#if NET9_0_OR_GREATER
    [Fact]
    public async Task ResolveListReferences_DeepStructure()
    {
        NexYamlSerializerRegistry.Init();
        var list = new ReferenceScriptList();
        Guid guid = Guid.NewGuid();
        Guid guid2 = Guid.NewGuid();
        Guid guid3 = Guid.NewGuid();

        var refScript2 = new ReferenceScript()
        {
            Id = guid2,
            Reference = new ReferenceClass()
            {
            },
        };

        var refScript = new ReferenceScript()
        {
            Id= guid,
            Reference = new ReferenceClass()
            {
                ReferenceScript =  refScript2
            },
        };

        var refScript3 = new ReferenceScript()
        {
            Id = guid3,
            Reference = new ReferenceClass()
            {
                ReferenceScript = refScript
            },
        };
        list.List.Add(refScript);
        list.List.Add(refScript2);
        list.List.Add(refScript3);
        var s = Yaml.Write(list);
        var d = await Yaml.Read<ReferenceScriptList>(s);
        Assert.Equal(d.List[1], d.List[0].Reference.ReferenceScript);
    }
#endif
    [Fact]
    public async Task StructsLinkedWithinReference()
    {
        NexYamlSerializerRegistry.Init();
        var a = new ClassA();
        a.Id = Guid.NewGuid();
        a.MyStruct = new MyStruct() { MyRef = new ClassB() { Id = a.Id } };
        var s = Yaml.Write<ClassA>(a);
        var d = await Yaml.Read<ClassA>(s);
        Assert.NotNull(d);
        Assert.Equal(a.Id, d.Id);
        Assert.Equal(a.MyStruct.MyRef.Id,d.MyStruct.MyRef.Id);
    }
}
