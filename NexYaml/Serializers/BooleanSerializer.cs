using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
namespace NexYaml.Serializers;

public class BooleanSerializer : IYamlSerializer<bool>
{
    public void Write<X>(WriteContext<X> context, bool value, DataStyle style) where X : Node
    {
        context.WriteScalar(value ? ['t', 'r', 'u', 'e'] : ['f', 'a', 'l', 's', 'e']);
    }

    public ValueTask<bool> Read(Scope scope, bool parseResult)
    {
        var s = scope.As<ScalarScope>();
        return new(bool.Parse(s.Value));
    }
}
