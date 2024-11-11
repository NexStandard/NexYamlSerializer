using NexVYaml.Parser;
using NexYaml;
using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.Formatters;

public class NullableStringFormatter : YamlSerializer<string?>
{
    public static readonly NullableStringFormatter Instance = new();
    public override void Write(IYamlWriter stream, string? value, DataStyle style)
    {
        stream.Write(value!);
    }

    public override void Read(IYamlReader parser, ref string? value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            value = StringEncoding.Utf8.GetString(span);
            parser.ReadWithVerify(ParseEventType.Scalar);
            return;
        }
        value = null;
    }
}

