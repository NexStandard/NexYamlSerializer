using NexYaml.Parser;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;

internal class NullablePlugin : IResolvePlugin
{
    private static readonly Type NullableSerializer = typeof(NullableSerializer<>);
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        return false;
    }

    public bool Write<T>(IYamlWriter stream, T value, DataStyle provider)
    {
        var type = typeof(T);
        Type? targetType = null;
        if ((targetType = Nullable.GetUnderlyingType(type)) is not null)
        {
            stream.Resolver.GetSerializer(targetType)?.Write(stream, value);
            return true;
        }
        return false;
    }
}