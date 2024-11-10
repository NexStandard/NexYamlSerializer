using Stride.Core;

namespace NexYaml.Serialization.Formatters;
public class EmptyFormatter<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS()
    {
        return new EmptyFormatter<T>();
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        stream.Write([(byte)'!', (byte)'!', (byte)'n', (byte)'u', (byte)'l', (byte)'l']);
    }

    public override void Read(IYamlReader parser, ref T value)
    {
        value = default!;
    }
}
