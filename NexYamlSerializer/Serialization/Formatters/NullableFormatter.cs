#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableFormatter<T> : YamlSerializer<T?> where T : struct
{
    public override void Write(IYamlWriter stream, T? value, DataStyle style)
    {
        if (value is null)
        {
            stream.Write("!!null");
        }
        else
        {
            stream.Write(value.Value, style);
        }
    }

    public override void Read(IYamlReader parser, ref T? value)
    {
        var val = default(T);
        parser.Read(ref val);
        value =  new T?(val);
    }
}
public sealed class StaticNullableFormatter<T>(YamlSerializer<T> underlyingFormatter) : YamlSerializer<T?> where T : struct
{
    readonly YamlSerializer<T> underlyingSerializer = underlyingFormatter;

    public override void Write(IYamlWriter stream, T? value, DataStyle style)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Write(stream, value.Value, style);
        }
        else
        {
            stream.Write("!!null");
        }
    }

    public override void Read(IYamlReader parser, ref T? value)
    {
        var val = default(T);
        parser.Read(ref val);
        value = val;
    }
}
