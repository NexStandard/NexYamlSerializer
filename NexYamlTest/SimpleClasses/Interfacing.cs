using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class Interfacing : IDInterface
{
    public int Id { get; set; }
}
interface IDInterface
{
    public int Id { get; set; }
}
