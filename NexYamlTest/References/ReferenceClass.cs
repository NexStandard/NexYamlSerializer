using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.References;

[DataContract]
internal class ClassA : ClassB,IIdentifiable { public MyStruct MyStruct;

}
[DataContract]
class ClassB : IIdentifiable
{
    public Guid Id { get; set; }
}
[DataContract]
internal struct MyStruct { public ClassB MyRef; }
[DataContract]
internal class ReferenceClass : IIdentifiable
{
    public Guid Id { get; set; }
    public ReferenceScript ReferenceScript { get; set; }
    public int Test {  get; set; }
}
[DataContract]
internal record ReferenceRecordScript
{
    public ReferenceClass Reference { get; set; }
    public ReferenceClass Reference1 { get; set; }
    public ReferenceClass Reference2 { get; set; }
}
[DataContract]
internal class ReferenceScript : IIdentifiable
{
    public ReferenceClass Reference { get; set; }
    public ReferenceClass Reference1 { get; set; }
    public ReferenceClass Reference2 { get; set; }
    public Guid Id { get; set; }
}