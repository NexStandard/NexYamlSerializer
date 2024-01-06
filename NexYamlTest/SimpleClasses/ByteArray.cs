using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class ByteArray
{
    public byte[] Data { get; set; } = [1 , 2 ,3 ,4 ];
}
