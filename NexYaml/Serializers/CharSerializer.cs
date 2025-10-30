using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : IYamlSerializer<char>
{
    public void Write<X>(WriteContext<X> context, char value, DataStyle style) where X : Node
    {
        context.WriteScalar(['\'', value, '\'']);
    }

    public ValueTask<char> Read(Scope scope, char parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(char.Parse(scalarScope.Value));
    }
}
