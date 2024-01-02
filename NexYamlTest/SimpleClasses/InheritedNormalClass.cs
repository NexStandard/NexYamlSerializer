using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class InheritedNormalClass : BaseClass
{
    public int Test {  get; set; }
}
[DataContract]
internal class BaseClass
{
    public string Name { get; set; }
}
