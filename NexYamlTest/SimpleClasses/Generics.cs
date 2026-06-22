using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal record class Generics<T>
{
    public T? Value { get; set; }
}
[DataContract]
internal class GenericWithRestriction<T>
    where T : class, new()
{
    public T? Value { get; init; }
}
[DataContract]
internal record class GenericWithImplementation : Generics<int>
{

}