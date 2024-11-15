using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.References;
[DataContract]
internal class ReferenceData : IIdentifiable
{
    public Guid Id { get; set; }
    public int Test {  get; set; }
}
[DataContract]
internal class ReferenceScript
{
    public ReferenceData Reference { get; set; }
    public ReferenceData Reference1 { get; set; }
    public ReferenceData Reference2 { get; set; }
}