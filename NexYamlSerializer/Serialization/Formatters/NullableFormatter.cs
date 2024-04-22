#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableFormatter<T> : YamlSerializer<T?>, IYamlFormatter<T?> where T : struct
{
    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        var value = default(T);
        context.DeserializeWithAlias(ref parser, ref value);
        var x = new T?(value);
        return x;
    }

    public override void Serialize(ISerializationWriter stream, T? value, DataStyle style = DataStyle.Normal)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.Write(value.Value);
        }
    }
}
public sealed class StaticNullableFormatter<T>(YamlSerializer<T> underlyingFormatter) : YamlSerializer<T?>, IYamlFormatter<T?> where T : struct
{
    readonly YamlSerializer<T> underlyingSerializer = underlyingFormatter;

    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        var value = default(T);
        context.DeserializeWithAlias(ref parser,ref value);
        return value;
    }

    public override void Serialize(ISerializationWriter stream, T? value, DataStyle style = DataStyle.Normal)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Serialize(ref stream, value.Value, style);
        }
        else
        {
            stream.Write(YamlCodes.Null0);
        }
    }
}
