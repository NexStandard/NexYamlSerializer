using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class InitClass
{
    public required EmptyClass EmptyClass { get; init; }
}
