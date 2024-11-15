using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;

internal class NullPlugin : IResolvePlugin
{
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        if (parser.IsNullScalar())
        {
            value = default;
            parser.Move();
            return true;
        }
        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is null)
        {
            stream.WriteScalar(stream.Settings.Null);
            return true;
        }
        return false;
    }
}