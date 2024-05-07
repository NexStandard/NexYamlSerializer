#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt64Formatter : YamlSerializer<ulong>
{
    public static readonly UInt64Formatter Instance = new();

    public override void Serialize(ISerializationWriter stream, ulong value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref ulong value)
    {
        var result = parser.GetScalarAsUInt64();
        parser.Read();
        value = result;
    }
}
