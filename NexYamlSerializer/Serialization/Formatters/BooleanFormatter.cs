#nullable enable
using NexVYaml.Parser;
using NexYaml.Core;
using Stride.Core;
using System;
namespace NexVYaml.Serialization;

public class BooleanFormatter : YamlSerializer<bool>
{
    public static readonly BooleanFormatter Instance = new();

    protected override void Write(IYamlWriter stream, bool value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref bool value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            switch (span.Length)
            {
                case 4 when span.SequenceEqual(YamlCodes.True0) ||
                            span.SequenceEqual(YamlCodes.True1) ||
                            span.SequenceEqual(YamlCodes.True2):
                    value = true;
                    break;

                case 5 when span.SequenceEqual(YamlCodes.False0) ||
                            span.SequenceEqual(YamlCodes.False1) ||
                            span.SequenceEqual(YamlCodes.False2):
                    value = false;
                    break;
            }
        }
        parser.Read();
    }
}
