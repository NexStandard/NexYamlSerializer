using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : YamlSerializer<Uri>
{
    public static readonly UriSerializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, Uri value, DataStyle style, in WriteContext context)
    {
        return context.Write(value.ToString());
    }

    public override void Read(IYamlReader stream, ref Uri value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            stream.Move();
            value = uri;
        }
    }
}
