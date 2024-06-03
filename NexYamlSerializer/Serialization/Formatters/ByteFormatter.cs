#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class ByteFormatter : YamlSerializer<byte>
{
    public static readonly ByteFormatter Instance = new();

    protected override void Write(ISerializationWriter stream, byte value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref byte value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((byte)result);
    }
}
