using NexYaml.Core;
using NexYaml.Parser;
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
