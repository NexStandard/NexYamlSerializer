using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Interfacing : IDInterface
{
    public int Id { get; set; }
}

internal interface IDInterface
{
    public int Id { get; set; }
}
