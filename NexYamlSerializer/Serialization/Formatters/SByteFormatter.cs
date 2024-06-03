#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class SByteFormatter : YamlSerializer<sbyte>
{
    public static readonly SByteFormatter Instance = new();

    protected override void Write(ISerializationWriter stream, sbyte value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref sbyte value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = checked((sbyte)result);
    }
}
