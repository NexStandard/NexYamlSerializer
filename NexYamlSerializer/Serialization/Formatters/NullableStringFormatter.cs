#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableStringFormatter : YamlSerializer<string?>
{
    public static readonly NullableStringFormatter Instance = new NullableStringFormatter();
    public override void Serialize(ISerializationWriter stream, string? value, DataStyle style)
    {
        stream.Serialize(ref value!);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref string? value)
    {
        value = parser.ReadScalarAsString();
    }
}

