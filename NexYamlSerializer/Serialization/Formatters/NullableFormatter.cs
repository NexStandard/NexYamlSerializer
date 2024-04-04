#nullable enable
using NexVYaml;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class NullableFormatter<T> : YamlSerializer<T?>, IYamlFormatter<T?> where T : struct
{
    IYamlFormatter<T> yamlFormatter;
    public NullableFormatter(IYamlFormatter<T> formatter) { yamlFormatter = formatter; }
    public NullableFormatter() { }

    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }

        return new T?(yamlFormatter.Deserialize(ref parser,context));
    }

    public override void Serialize(ref IYamlStream stream, T? value, DataStyle style = DataStyle.Normal)
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
public sealed class StaticNullableFormatter<T> : YamlSerializer<T?>, IYamlFormatter<T?> where T : struct
{
    readonly IYamlFormatter<T> underlyingFormatter;
    readonly YamlSerializer<T> underlyingSerializer;

    public StaticNullableFormatter(YamlSerializer<T> underlyingFormatter)
    {
        underlyingSerializer = underlyingFormatter;
    }
    public StaticNullableFormatter(IYamlFormatter<T> underlyingFormatter)
    {
        this.underlyingFormatter = underlyingFormatter;
    }

    public override T? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return null;
        }
        return underlyingFormatter.Deserialize(ref parser, context);
    }

    public override void Serialize(ref IYamlStream stream, T? value, DataStyle style = DataStyle.Normal)
    {
        if (value.HasValue)
        {
            underlyingSerializer.Serialize(ref stream,value.Value,style);
        }
        else
        {
            stream.WriteNull();
        }
    }
}
