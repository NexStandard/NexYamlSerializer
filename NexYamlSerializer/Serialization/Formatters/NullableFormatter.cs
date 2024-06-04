#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableFormatter<T> : YamlSerializer<T?> where T : struct
{
    protected override void Write(ISerializationWriter stream, T? value, DataStyle style)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.Write(value.Value, style);
        }
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref T? value)
    {
        var val = default(T);
        context.DeserializeWithAlias(ref parser, ref val);
        value =  new T?(val);
    }
}
public sealed class StaticNullableFormatter<T>(YamlSerializer<T> underlyingFormatter) : YamlSerializer<T?> where T : struct
{
    readonly YamlSerializer<T> underlyingSerializer = underlyingFormatter;

    protected override void Write(ISerializationWriter stream, T? value, DataStyle style)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Serialize(stream, value.Value, style);
        }
        else
        {
            stream.WriteNull();
        }
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref T? value)
    {
        var val = default(T);
        context.DeserializeWithAlias(ref parser, ref val);
        value = val;
    }
}
