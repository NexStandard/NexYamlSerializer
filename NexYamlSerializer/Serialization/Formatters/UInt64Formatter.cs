#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt64Formatter : YamlSerializer<ulong>
{
    public static readonly UInt64Formatter Instance = new();

    public override ulong Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt64();
        parser.Read();
        return result;
    }

    public override void Serialize(ref ISerializationWriter stream, ulong value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
