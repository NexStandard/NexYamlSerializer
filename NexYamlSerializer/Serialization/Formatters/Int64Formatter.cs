#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>, IYamlFormatter<long>
{
    public static readonly Int64Formatter Instance = new();

    public override long Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt64();
        parser.Read();
        return result;
    }

    public override void Serialize(ref ISerializationWriter stream, long value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
