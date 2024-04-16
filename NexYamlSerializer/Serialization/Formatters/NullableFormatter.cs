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

        return new T?(context.DeserializeWithAlias<T>(ref parser));
    }

    public override void Serialize(ISerializationWriter stream, T? value, DataStyle style = DataStyle.Normal)
    {
        if (value is null)
        {
            stream.WriteNull();
        }
        else
        {
            stream.Serialize(value.Value);
        }
    }
}
public sealed class StaticNullableFormatter<T> : YamlSerializer<T?>, IYamlFormatter<T?> where T : struct
{
    readonly YamlSerializer<T> underlyingSerializer;

    public StaticNullableFormatter(YamlSerializer<T> underlyingFormatter)
    {
        underlyingSerializer = underlyingFormatter;
    }

    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        return context.DeserializeWithAlias<T>(ref parser);
    }

    public override void Serialize(ISerializationWriter stream, T? value, DataStyle style = DataStyle.Normal)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Serialize(ref stream, value.Value, style);
        }
        else
        {
            stream.Serialize(YamlCodes.Null0);
        }
    }
}
