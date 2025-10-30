using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : YamlSerializer<char>
{
    public override void Write<X>(WriteContext<X> context, char value, DataStyle style)
    {
        context.WriteScalar(['\'', value, '\'']);
    }

    public override ValueTask<char> Read(Scope scope, char parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(char.Parse(scalarScope.Value));
    }
}
