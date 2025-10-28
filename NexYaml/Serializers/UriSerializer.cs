using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UriSerializer : YamlSerializer<Uri>
{
    public override void Write<X>(WriteContext<X> context, Uri value, DataStyle style)
    {
        context.WriteScalar(value.ToString());
    }

    public override ValueTask<Uri?> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(new Uri(scalarScope.Value, UriKind.RelativeOrAbsolute));
    }
}
