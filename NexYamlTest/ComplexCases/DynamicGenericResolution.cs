using Stride.Core;

namespace NexYamlTest.ComplexCases;
[DataContract]
internal class GenericImplementedClass<T, K> : IGenericInterface<T, K>
{
    public T? Generic { get; set; }
    public K? Generic2 { get; set; }
}
[DataContract]
internal class GenericImplementedClassWithLessParams<T> : IGenericInterface<T, int>
{
    public T? Generic { get; set; }
    public int Generic2 { get; set; }
}
[DataContract]
internal class SubstitutedGenericClassNoParams : GenericImplementedClassWithLessParams<int>
{

}
abstract class GenericAbstract<T,K>
{
    public int Test { get; set; }
}
[DataContract]
abstract class GenericAbstractWithDataContract<T, K>
{
    public int Test { get; set; }
}
[DataContract]
class GenericAbstractImplementation<T,K> : GenericAbstract<T, K>
{
    public required T TI { get; set; }
    public required K TI2 { get; set; }
}
[DataContract]
class GenericAbstractImlementationLessParams<T> : GenericAbstract<T, int>
{
    public int X;
}
[DataContract]
class GenericAbstractImlementationLessParamsEmpty<T> : GenericAbstract<T, int>
{
}
[DataContract]
class GenericAbstractImlementationLessParamsDataContract<T> : GenericAbstractWithDataContract<T, int>
{

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
[DataContract]
class Base
{
    public int X;
}
[DataContract]
class Inherited : Base
{
    public double T;
}