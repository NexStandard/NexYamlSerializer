#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableStringFormatter : YamlSerializer<string?>,IYamlFormatter<string?>
{
    public static readonly NullableStringFormatter Instance = new();

    public void Serialize(ref Utf8YamlEmitter emitter, string? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        if (value == null)
        {
            emitter.WriteNull();
        }
        else
        {
            emitter.WriteString(value);
        }
    }

    public override string? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        return parser.ReadScalarAsString();
    }

    public override void Serialize(ref IYamlStream stream, string? value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value!);
    }
}

