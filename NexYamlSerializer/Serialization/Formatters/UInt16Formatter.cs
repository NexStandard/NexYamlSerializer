#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt16Formatter : YamlSerializer<ushort>
{
    public static readonly UInt16Formatter Instance = new();

    public override void Serialize(ISerializationWriter stream, ushort value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ushort value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((ushort)result);
    }
}
