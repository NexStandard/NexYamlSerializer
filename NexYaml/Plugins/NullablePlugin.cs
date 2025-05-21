using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;

internal class NullablePlugin : IResolvePlugin
{
    public bool Write<T, X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        var type = typeof(T);
        Type? targetType = null;
        if ((targetType = Nullable.GetUnderlyingType(type)) is not null)
        {
            context.Writer.Resolver.GetSerializer(targetType, targetType)!.Write(context, value, style);
            return true;
        }
        return false;
    }
    public bool Read<T>(IYamlReader stream, out ValueTask<T?> value, ParseContext result)
    {
        // TODO: handling for reading nullables
        value = default;
        return false;
    }
}