#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int32Formatter : YamlSerializer<int>, IYamlFormatter<int>
{
    public static readonly Int32Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, int value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public override int Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return result;
    }

    public override void Serialize(ref IYamlStream stream, int value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
