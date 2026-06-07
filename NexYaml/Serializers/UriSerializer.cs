using NexYaml.Parser;
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
        var scalarScope = scope.As<ScalarScope>();
        return new(new Uri(scalarScope.Value, UriKind.RelativeOrAbsolute));
    }
}
