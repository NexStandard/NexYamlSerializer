using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;

internal class NullPlugin : IResolvePlugin
{
    public bool Write<T, X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
            return true;
        }
        return false;
    }

    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (stream.IsNullScalar())
        {
            value = default;
            stream.Move();
            return true;
        }
        return false;
    }

    public bool Read<T>(IYamlReader stream, T value, ParseContext<T> result)
    {
        if (stream.IsNullScalar())
        {
            value = default;
            stream.Move();
            return true;
        }
        return false;
    }
}