
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : IYamlSerializer<char>
{
    public void Write(Node node, char value, DataStyle style)
    {
        node.WriteScalar(['\'', value, '\'']);
    }

    public ValueTask<char> Read(Scope scope, char parseResult)
    {
        return new(scope.AsScalar()[0]);
    }
}
