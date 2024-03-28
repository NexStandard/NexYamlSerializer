#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class UInt32Formatter : IYamlFormatter<uint>
{
    public static readonly UInt32Formatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, uint value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        emitter.WriteUInt32(value);
    }

    public uint Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        return result;
    }
}
