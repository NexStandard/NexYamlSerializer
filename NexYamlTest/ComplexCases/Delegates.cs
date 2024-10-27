using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public void Test() {
        
    }
}
