using Stride.Core;

namespace NexYamlTest.SimpleClasses;
[DataContract]
internal class ByteArray
{
    public byte[] Data { get; set; } = [1 , 2 ,3 ,4 ];
}
