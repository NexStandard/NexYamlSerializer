#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableFormatter<T> : YamlSerializer<T?> where T : struct
{
    protected override void Write(IYamlWriter stream, T? value, DataStyle style)
    {
        if (value is null)
        {
            stream.Write(YamlCodes.Null0);
        }
        else
        {
            stream.Write(value.Value, style);
        }
    }

    protected override void Read(IYamlReader parser, ref T? value)
    {
        var val = default(T);
        parser.Read(ref val);
        value =  new T?(val);
    }
}
public sealed class StaticNullableFormatter<T>(YamlSerializer<T> underlyingFormatter) : YamlSerializer<T?> where T : struct
{
    readonly YamlSerializer<T> underlyingSerializer = underlyingFormatter;

    protected override void Write(IYamlWriter stream, T? value, DataStyle style)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Serialize(stream, value.Value, style);
        }
        else
        {
            stream.Write(YamlCodes.Null0);
        }
    }

    protected override void Read(IYamlReader parser, ref T? value)
    {
        var val = default(T);
        parser.Read(ref val);
        value = val;
    }
}
