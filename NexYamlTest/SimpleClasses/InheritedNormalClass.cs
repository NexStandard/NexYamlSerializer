using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class InheritedNormalClass : BaseClass
{
    public int Test {  get; set; }
    public override int AbstractInt { get; set; }
    public override string Name { get; set; }
}
[DataContract]
internal abstract class BaseClass
{
    public abstract int AbstractInt { get; set; }
    public virtual string Name { get; set; }
}
