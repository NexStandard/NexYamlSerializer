#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableStringFormatter : YamlSerializer<string?>
{
    public static readonly NullableStringFormatter Instance = new ();
    protected override void Write(IYamlWriter stream, string? value, DataStyle style)
    {
        stream.Write(value!);
    }

    protected override void Read(IYamlReader parser, ref string? value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            value = StringEncoding.Utf8.GetString(span);
            parser.ReadWithVerify(ParseEventType.Scalar);
            return;
        }
        value = null;
    }
}

