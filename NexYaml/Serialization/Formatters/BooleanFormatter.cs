using NexYaml.Core;
using Stride.Core;
using System;
namespace NexYaml.Serialization.Formatters;

public class BooleanFormatter : YamlSerializer<bool>
{
    public static readonly BooleanFormatter Instance = new();

    public override void Write(IYamlWriter stream, bool value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref bool value)
    {
        if (parser.TryGetScalarAsSpan(out var span))
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
        parser.Move();
    }
}
