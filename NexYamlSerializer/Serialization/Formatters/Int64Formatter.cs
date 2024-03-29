#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int64Formatter : YamlSerializer<long>, IYamlFormatter<long>
{
    public static readonly Int64Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, long value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt64(value);
    }

    public override long Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt64();
        parser.Read();
        return result;
    }

    public override void Serialize(ref IYamlStream stream, long value, DataStyle style = DataStyle.Normal)
    {
        stream.Write(ref value);
    }
}
