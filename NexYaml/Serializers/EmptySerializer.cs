using NexYaml.Core;
using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;
public class EmptySerializer<T> : IYamlSerializer<T>
{
    public static IYamlSerializer<T> EmptyS()
    {
        return new EmptySerializer<T>();
    }

    public void Write(Node context, T value, DataStyle style)
    {
        context.WriteScalar(YamlCodes.Null);
    }

    public ValueTask<T> Read(Scope scope, T? parseResult)
    {
        return default;
    }
}
