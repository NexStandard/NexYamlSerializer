using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
[DataContract]
public class TypeDictionary
{
    public Dictionary<Type, int> TypeMap = new Dictionary<Type, int>()
    {
        { typeof(string), 0 },
        { typeof(int), 1 },
        { typeof(double), 2 },
    };
}
