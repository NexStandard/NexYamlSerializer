
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : IYamlSerializer<Uri>
{
    public void Write(Node context, Uri value, DataStyle style)
    {
        context.WriteScalar(value.ToString());
    }

    public ValueTask<Uri> Read(Scope scope, Uri? parseResult)
    {
        return new(new Uri(scope.AsScalar().ToString(), UriKind.RelativeOrAbsolute));
    }
}
