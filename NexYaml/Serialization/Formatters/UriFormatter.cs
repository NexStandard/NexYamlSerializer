using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.Formatters;

public class UriFormatter : YamlSerializer<Uri>
{
    public static readonly UriFormatter Instance = new();

    public override void Write(IYamlWriter stream, Uri value, DataStyle style)
    {
        stream.Write(value.ToString());
    }

    public override void Read(IYamlReader parser, ref Uri value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            parser.Move();
            value = uri;
        }
    }
}
