using NexYaml.Parser;
using NexYaml.Serialization.Formatters;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;

internal class NullablePlugin : IResolvePlugin
{
    private static readonly Type NullableFormatter = typeof(NullableFormatter<>);
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        var type = typeof(T);
        Type underlyingType;
        if ((underlyingType = Nullable.GetUnderlyingType(type)) != null)
        {
            var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericFilledFormatter)!;
            f.Read(parser, ref value, ref result);
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
            var formatter = stream.Resolver.GetFormatter(targetType);
            formatter.Write(stream, value);
            return true;
        }
        return false;
    }
}