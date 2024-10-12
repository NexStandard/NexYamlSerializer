#nullable enable
using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;

namespace NexYamlSerializer.Serialization.PrimitiveSerializers;

public class CharFormatter : YamlSerializer<char>
{
    public static readonly CharFormatter Instance = new();

    protected override void Write(IYamlWriter stream, char value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref char value)
    {
        if(parser.TryGetScalarAsString(out var result))
        {
            if (result is not null && result.Length == 1)
            {
                if (result.Length == 1)
                {
                    value = result[0];
                }
            }
        }
        
        parser.Read();
    }
}
