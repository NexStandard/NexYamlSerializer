#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;

namespace NexVYaml.Serialization;

public class UriFormatter : YamlSerializer<Uri>
{
    public static readonly UriFormatter Instance = new();

    public override void Serialize(ISerializationWriter stream, Uri value, DataStyle style)
    {
        stream.Write(value.ToString());
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref Uri value)
    {
        if (parser.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            parser.Read();
            value = uri;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of Uri : {parser.CurrentEventType} {parser.GetScalarAsString()}");
    }
}
