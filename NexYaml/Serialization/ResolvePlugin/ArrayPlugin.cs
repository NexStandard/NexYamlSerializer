using NexYaml.Parser;
using NexYaml.Serialization.Formatters;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class ArrayPlugin : IResolvePlugin
{


    public bool Write<T>(IYamlWriter stream, T value, DataStyle style)
    {
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Write(stream, value, style);
            return true;
        }
        return false;
    }
    public bool Read<T>(IYamlReader parser, ref T value, ref ParseResult result)
    {
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Read(parser, ref val, ref result);
            value = (T)val!;
            return true;
        }
        return false;
    }
}
