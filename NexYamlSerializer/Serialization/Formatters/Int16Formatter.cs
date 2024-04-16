#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int16Formatter : YamlSerializer<short>, IYamlFormatter<short>
{
    public static readonly Int16Formatter Instance = new();

    public override short Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return checked((short)result);
    }

    public override void Serialize(ref ISerializationWriter stream, short value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
