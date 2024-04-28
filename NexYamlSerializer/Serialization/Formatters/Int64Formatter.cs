#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>
{
    public static readonly Int64Formatter Instance = new();

    public override long Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt64();
        parser.Read();
        return result;
    }

    public override void Serialize(ISerializationWriter stream, long value, DataStyle style)
    {
        stream.Serialize(ref value);
    }
}
