#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;

namespace NexVYaml.Serialization;

public class UriFormatter : YamlSerializer<Uri>
{
    public static readonly UriFormatter Instance = new();

    protected override void Write(IYamlWriter stream, Uri value, DataStyle style)
    {
        stream.Write(value.ToString());
    }

    protected override void Read(YamlParser parser, ref Uri value)
    {
        if (parser.TryGetScalarAsString(out var scalar) && scalar != null)
        {
            var uri = new Uri(scalar, UriKind.RelativeOrAbsolute);
            parser.Read();
            value = uri;
        }
    }
}
