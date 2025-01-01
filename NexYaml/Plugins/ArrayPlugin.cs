using NexYaml.Parser;
using NexYaml.Serializers;
using Stride.Core;

namespace NexYaml.Plugins;
internal class ArrayPlugin : IResolvePlugin
{
    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
            var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

            arraySerializer.Write(stream, value, style);
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

            arraySerializer.Read(stream, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }
}
