using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : IYamlSerializer<char>
{
    public void Write(Node context, char value, DataStyle style)
    {
        context.WriteScalar(['\'', value, '\'']);
    }

    public ValueTask<char> Read(Scope scope, char parseResult)
    {
        return new(scope.AsScalar()[0]);
    }
}
