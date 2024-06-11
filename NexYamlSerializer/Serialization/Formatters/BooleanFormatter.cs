#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class BooleanFormatter : YamlSerializer<bool>
{
    public static readonly BooleanFormatter Instance = new();

    protected override void Write(IYamlWriter stream, bool value, DataStyle style)
    {
        stream.Serialize(value ? YamlCodes.True0 : YamlCodes.False0);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref bool value)
    {
        var result = parser.GetScalarAsBool();
        parser.Read();
        value = result;
    }
}
