using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : YamlSerializer<Uri>
{
    public static readonly UriSerializer Instance = new();

    public override void Write(IYamlWriter stream, Uri value, DataStyle style)
    {
        stream.Write(value.ToString());
    }

    public override void Read(IYamlReader stream, ref Uri value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            stream.Read();
            value = uri;
        }
    }
}
