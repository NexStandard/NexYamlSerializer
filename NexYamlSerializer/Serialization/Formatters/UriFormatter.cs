#nullable enable
using System;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class UriFormatter : YamlSerializer<Uri>,IYamlFormatter<Uri>
{
    public static readonly UriFormatter Instance = new();

    public override Uri Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            parser.Read();
            return uri;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of Uri : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }

    public override void Serialize(ref ISerializationWriter stream, Uri value, DataStyle style = DataStyle.Normal)
    {
        stream.Write(value.ToString());
    }
}
