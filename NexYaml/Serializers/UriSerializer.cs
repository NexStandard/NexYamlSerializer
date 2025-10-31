using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : IYamlSerializer<Uri>
{
    public void Write<X>(WriteContext<X> context, Uri value, DataStyle style) where X : Node
    {
        context.WriteScalar(value.ToString());
    }

    public ValueTask<Uri> Read(Scope scope, Uri? parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(new Uri(scalarScope.Value, UriKind.RelativeOrAbsolute));
    }
}
