#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int16Formatter : YamlSerializer<short>
{
    public static readonly Int16Formatter Instance = new();

    protected override void Write(SerializationWriter stream, short value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref short value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = checked((short)result);
    }
}
