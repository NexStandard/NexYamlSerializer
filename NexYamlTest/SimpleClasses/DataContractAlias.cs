using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract("Alias")]
internal class DataContractAlias : IDInterface
{
    public int Id { get; set; }
}
