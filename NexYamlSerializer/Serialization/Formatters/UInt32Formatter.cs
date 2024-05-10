#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt32Formatter : YamlSerializer<uint>
{
    public static readonly UInt32Formatter Instance = new();

    protected override void Write(SerializationWriter stream, uint value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref uint value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = result;
    }
}
