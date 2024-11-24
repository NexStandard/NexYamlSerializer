using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;
public class EmptySerializer<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS()
    {
        return new EmptySerializer<T>();
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        stream.Write([(byte)'!', (byte)'!', (byte)'n', (byte)'u', (byte)'l', (byte)'l']);
    }

    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        value = default!;
    }
}
