using NexYaml.Parser;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;

internal class NullablePlugin : IResolvePlugin
{
    private static readonly Type NullableSerializer = typeof(NullableSerializer<>);
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        var type = typeof(T);
        Type underlyingType;
        if ((underlyingType = Nullable.GetUnderlyingType(type)) != null)
        {
            var genericSerializer = NullableSerializer.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericSerializer)!;
            f.Read(stream, ref value, ref result);
            return true;
        }
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