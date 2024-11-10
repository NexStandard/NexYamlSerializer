using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Abstracting : IDAbstract
{

}
[DataContract]
internal abstract class IDAbstract
{
    public int Id { get; set; }
}