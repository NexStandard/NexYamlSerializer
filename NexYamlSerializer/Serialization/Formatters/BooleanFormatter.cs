#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class BooleanFormatter : YamlSerializer<bool>
{
    public static readonly BooleanFormatter Instance = new();

    protected override void Write(ISerializationWriter stream, bool value, DataStyle style)
    {
        stream.Serialize(ref value);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref bool value)
    {
        var result = parser.GetScalarAsBool();
        parser.Read();
        value = result;
    }
}
