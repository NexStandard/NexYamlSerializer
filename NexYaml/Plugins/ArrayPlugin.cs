using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;
internal class ArrayPlugin : IResolvePlugin
{
    public bool Write<T,X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node
    {
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            arraySerializer.Write(context, value, style);
            return true;
        }
        return false;
    }
    public bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            arraySerializer.ReadUnknown(stream, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }

    public bool Read<T>(IYamlReader stream, T value, ParseContext result)
    {
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            // arraySerializer.ReadUnknown(stream, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }
}
