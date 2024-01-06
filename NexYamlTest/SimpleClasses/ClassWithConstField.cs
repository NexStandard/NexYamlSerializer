
using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class ClassWithConstField
{
    public const int Constant = 10;
    public int Normal = 0;
}
