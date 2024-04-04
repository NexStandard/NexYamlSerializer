#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class Int16Formatter : YamlSerializer<short>,IYamlFormatter<short>
{
    public static readonly Int16Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, short value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteInt32(value);
    }

    public override short Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        return checked((short)result);
    }

    public override void Serialize(ref IYamlStream stream, short value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }
}
