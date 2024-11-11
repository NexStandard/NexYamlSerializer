using NexYaml.Serialization.Formatters;
using Stride.Core;

namespace NexYaml.Serialization.SyntaxPlugins;
internal class ArrayPlugin : ISyntaxPlugin
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
}
