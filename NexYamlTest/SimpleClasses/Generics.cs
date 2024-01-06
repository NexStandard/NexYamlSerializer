using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Generics<T>
{
    public T Value { get; set; }
}
[DataContract]
internal class GenericWithRestriction<T>
    where T : class,new()
{
    public T Value { get; set; }
}
[DataContract]
internal class GenericWithImplementation : Generics<int>
{

}