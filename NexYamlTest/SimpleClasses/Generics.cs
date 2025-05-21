using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Generics<T>
{
    public T? Value { get; set; }
}
[DataContract]
internal class GenericWithRestriction<T>
    where T : class, new()
{
    public T? Value { get; set; }
}
[DataContract]
internal class GenericWithImplementation : Generics<int>
{

}