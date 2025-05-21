using System.Collections.Generic;
using Stride.Core;

namespace NexYamlTest.References;
[DataContract]
internal class ReferenceList
{
    public List<ReferenceClass> List { get; set; } = new();
}
[DataContract]
internal class ReferenceScriptList
{
    public List<ReferenceScript> List { get; set; } = new();
}