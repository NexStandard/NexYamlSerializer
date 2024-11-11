using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;

internal class NullablePlugin : ISyntaxPlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle provider)
    {
        var type = typeof(T);
        Type? targetType = null;
        if ((targetType = Nullable.GetUnderlyingType(type)) is not null)
        {
            var formatter = stream.Resolver.GetFormatter(targetType);
            formatter.Write(stream, value);
            return true;
        }
        return false;
    }
}