using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System;
using System.Globalization;

namespace NexYaml.Serialization.Formatters;

public class Float32Formatter : YamlSerializer<float>
{
    public static readonly Float32Formatter Instance = new();

    public override void Write(IYamlWriter stream, float value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref float value, ref ParseResult result)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            if (float.TryParse(span, CultureInfo.InvariantCulture, out value))
            {
                parser.Move();
                return;
            }

            switch (span.Length)
            {
                case 4:
                    if (span.SequenceEqual(YamlCodes.Inf0) ||
                        span.SequenceEqual(YamlCodes.Inf1) ||
                        span.SequenceEqual(YamlCodes.Inf2))
                    {
                        value = float.PositiveInfinity;
                        parser.Move();
                        return;
                    }

                    if (span.SequenceEqual(YamlCodes.Nan0) ||
                        span.SequenceEqual(YamlCodes.Nan1) ||
                        span.SequenceEqual(YamlCodes.Nan2))
                    {
                        value = float.NaN;
                        parser.Move();
                        return;
                    }
                    break;
                case 5:
                    if (span.SequenceEqual(YamlCodes.Inf3) ||
                        span.SequenceEqual(YamlCodes.Inf4) ||
                        span.SequenceEqual(YamlCodes.Inf5))
                    {
                        value = float.PositiveInfinity;
                        parser.Move();
                        return;
                    }
                    if (span.SequenceEqual(YamlCodes.NegInf0) ||
                        span.SequenceEqual(YamlCodes.NegInf1) ||
                        span.SequenceEqual(YamlCodes.NegInf2))
                    {
                        value = float.NegativeInfinity;
                        parser.Move();
                        return;
                    }
                    break;
            }
        }
    }
}
