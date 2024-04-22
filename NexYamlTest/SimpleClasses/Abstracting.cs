using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
class Abstracting : IDAbstract
{

}
[DataContract]
abstract class IDAbstract
{
    public int Id { get; set; }
}