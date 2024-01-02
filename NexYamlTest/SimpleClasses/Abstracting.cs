using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
[DataContract]
class Abstracting : IDAbstract
{

}
[DataContract]
abstract class IDAbstract
{
    public int Id { get; set; }
}