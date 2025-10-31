using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class GuidSerializer : IYamlSerializer<Guid>
{
    public void Write<X>(WriteContext<X> context, Guid value, DataStyle style) where X : Node
    {
        context.WriteString(value.ToString());
    }

    public ValueTask<Guid> Read(Scope scope, Guid parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(Guid.Parse(scalarScope.Value));
    }
}
