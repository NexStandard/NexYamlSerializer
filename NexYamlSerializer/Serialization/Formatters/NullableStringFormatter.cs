#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableStringFormatter : YamlSerializer<string?>
{
    public static readonly NullableStringFormatter Instance = new();

    public override string? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        return parser.ReadScalarAsString();
    }

    public override void Serialize(ISerializationWriter stream, string? value, DataStyle style = DataStyle.Normal)
    {
        stream.Serialize(ref value!);
    }
}

