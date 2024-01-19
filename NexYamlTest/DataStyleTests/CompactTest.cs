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
        var compact = new CompactArray();
        YamlHelper.Run(compact);
    }
}
