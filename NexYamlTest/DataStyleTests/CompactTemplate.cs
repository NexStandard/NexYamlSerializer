using System;
using System.Collections.Generic;
using Stride.Core;
namespace NexYamlTest.DataStyleTests;

[DataStyle(DataStyle.Compact)]
[DataContract]
internal record class CompactClass
{
    public int X { get; set; }
    public int Y { get; set; }
    public string W { get; set; } = string.Empty;
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
    public string? W { get; set; }
}
[DataContract]
internal record class CompactCompactRecord
{
    [DataStyle(DataStyle.Compact)]
    public IEquatable<CompactRecordWithCompactMember> CompactMember = new CompactRecordWithCompactMember();
}
[DataContract]
[DataStyle(DataStyle.Compact)]
internal record class CompactRecordWithCompactMember
{
    public IEquatable<CompactRecord> CompactMember { get; set; } = new CompactRecord();
}
[DataContract]
internal record class CompactMembers
{
    public required CompactRecord X { get; set; }
    [DataStyle(DataStyle.Compact)]
    public required NonCompactClass NonCompactClass { get; set; }
}
[DataContract]
internal record class CompactList
{
    [DataStyle(DataStyle.Compact)]
    public List<NonCompactClass> Lists { get; set; } =
    [
        new NonCompactClass(),
        new NonCompactClass()
    ];
    public int Count { get; set; }
}
[DataContract]
internal record class NonCompactClass
{
    public int Y { get; set; }
    public string W { get; set; } = string.Empty;
}
[DataContract]
internal record class CompactArray
{
    [DataStyle(DataStyle.Compact)]
    public int[] Ints { get; set; } = [1, 2, 3];
}