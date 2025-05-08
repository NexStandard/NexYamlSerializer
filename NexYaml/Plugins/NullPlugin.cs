using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;

internal class NullPlugin : IResolvePlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle style, WriteContext context, out WriteContext newContext)
    {
        if (value is null)
        {
            newContext = context.WriteScalar(YamlCodes.Null);
            return true;
        }
        newContext = default;
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