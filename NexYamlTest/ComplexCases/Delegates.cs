using Stride.Core;
using System;

namespace NexYamlTest.ComplexCases;
[DataContract]
public class Delegates : IIdentifiable
{
    [DataMember]
    public Action Action { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();

    public Delegates()
    {
        Action = Test;
    }
    public void Test()
    {

    }
}
