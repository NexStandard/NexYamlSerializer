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

internal abstract class GenericAbstract<T, K>
{
    public int Test { get; set; }
}
[DataContract]
internal abstract class GenericAbstractWithDataContract<T, K>
{
    public int Test { get; set; }
}
[DataContract]
internal class GenericAbstractImplementation<T, K> : GenericAbstract<T, K>
{
    public T TI { get; set; }
    public K TI2 { get; set; }
}
[DataContract]
internal class GenericAbstractImlementationLessParams<T> : GenericAbstract<T, int>
{
    public int X;
}
[DataContract]
internal class GenericAbstractImlementationLessParamsEmpty<T> : GenericAbstract<T, int>
{
}
[DataContract]
internal class GenericAbstractLessParams<T> : GenericAbstractWithDataContract<T, int>
{

}
[DataContract]
internal class GenericImplementedClassWithNoParams : IGenericInterface<int, int>
{
    public int Generic { get; set; }
    public int Generic2 { get; set; }
}

internal interface IGenericInterface<T, K>
{
    public T Generic { get; set; }
    public K Generic2 { get; set; }
}
[DataContract]
internal class Base
{
    public int X;
}
[DataContract]
internal class Inherited : Base
{
    public double T;
}