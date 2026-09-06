using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;
namespace NexYaml.Serializers;

public class BooleanSerializer : IYamlSerializer<bool>
{
    public void Write(Node node, bool value, DataStyle style)
    {
        node.WriteScalar(value ? ['t', 'r', 'u', 'e'] : ['f', 'a', 'l', 's', 'e']);
    }

    public ValueTask<bool> Read(Scope scope, bool parseResult)
    {
        return new(bool.Parse(scope.AsScalar()));
    }
}
