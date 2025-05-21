using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : YamlSerializer<Uri>
{
    public static readonly UriSerializer Instance = new();
    public override void Write<X>(WriteContext<X> context, Uri value, DataStyle style)
    {
        context.WriteScalar(value.ToString());
    }

    public override ValueTask<Uri?> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            stream.Move();
            return new(uri);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(Uri), scalar, stream.CurrentMarker);
    }
}
