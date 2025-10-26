using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;
public class EmptySerializer<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS()
    {
        return new EmptySerializer<T>();
    }

    public override void Write<X>(WriteContext<X> context, T value, DataStyle style)
    {
        context.WriteScalar(YamlCodes.Null);
    }

    public override ValueTask<T?> Read(IYamlReader stream, ParseContext parseResult)
    {
        return default;
    }
    public override ValueTask<T?> Read(Scope scope, ParseContext parseResult)
    {
        return default;
    }
}
