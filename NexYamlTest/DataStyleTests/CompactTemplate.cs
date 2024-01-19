using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core;
namespace NexYamlTest.DataStyleTests;

[DataStyle(DataStyle.Compact)]
[DataContract]
internal record class CompactClass
{
    public int X { get; set; }
    public int Y { get; set; }
    public string W { get; set; }
}
[DataStyle(DataStyle.Compact)]
[DataContract]
internal record struct CompactStruct
{
    public int X { get; set; }
    public int Y { get; set; }
    public string W { get; set; }
}
[DataStyle(DataStyle.Compact)]
[DataContract]
internal record class CompactRecord
{
    public int X { get; set; }
    public int Y { get; set; }
    public string W { get; set; }
}

[DataContract]
internal record class CompactMembers
{
    public required CompactRecord X { get; set; }
    [DataStyle(DataStyle.Compact)]
    public required NonCompactClass NonCompactClass { get; set; }
}
[DataContract]
internal record class NonCompactClass
{
    public int Y { get; set; }
    public string W { get; set; }
}