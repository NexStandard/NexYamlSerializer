#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class UInt64Formatter : YamlSerializer<ulong>, IYamlFormatter<ulong>
{
    public static readonly UInt64Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, ulong value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteUInt64(value);
    }

    public override ulong Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt64();
        parser.Read();
        return result;
    }

    public override void Serialize(ref IYamlStream stream, ulong value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value);
    }

}
