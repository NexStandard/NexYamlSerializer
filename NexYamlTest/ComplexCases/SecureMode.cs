using Stride.Core;

namespace NexYamlTest.ComplexCases;
[DataContract]
internal class SecureMode : IInSecure
{
    public int X;
}
internal interface IInSecure
{
}