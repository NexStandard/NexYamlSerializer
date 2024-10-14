#nullable enable
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableStringFormatter : YamlSerializer<string?>
{
    public static readonly NullableStringFormatter Instance = new ();
    protected override void Write(IYamlWriter stream, string? value, DataStyle style)
    {
        stream.Write(value!);
    }

    protected override void Read(YamlParser parser, ref string? value)
    {
        value = parser.ReadScalarAsString();
    }
}

