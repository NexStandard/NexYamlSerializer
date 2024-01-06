

using Stride.Core;

namespace NexYamlTest.ComplexCases;
[DataContract]
internal class GenericImplementedClass<T, K> : IGenericInterface<T, K>
{
    public T Generic { get; set; }
    public K Generic2 { get; set; }
}
[DataContract]
internal class GenericImplementedClassWithLessParams<T> : IGenericInterface<T, int>
{
    public T Generic { get; set; }
    public int Generic2 { get; set; }
}
[DataContract]
internal class GenericImplementedClassWithNoParams : IGenericInterface<int, int>
{
    public int Generic { get; set; }
    public int Generic2 { get; set; }
}
interface IGenericInterface<T,K>
{
    public T Generic { get; set; }
    public K Generic2 { get; set; }
}