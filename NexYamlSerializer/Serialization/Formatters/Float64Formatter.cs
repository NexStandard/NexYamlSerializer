#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Globalization;
using System;
using NexYaml.Core;

namespace NexVYaml.Serialization;

public class Float64Formatter : YamlSerializer<double>
{
    public static readonly Float64Formatter Instance = new();

    protected override void Write(IYamlWriter stream, double value, DataStyle style)
    {
        stream.Write(value, style);
    }

    protected override void Read(YamlParser parser, ref double value)
    {
        if(parser.TryGetScalarAsSpan(out var span))
        {
            if (double.TryParse(span, CultureInfo.InvariantCulture, out value))
            {
                parser.Read();
                return;
            }

            switch (span.Length)
            {
                case 4:
                    if (span.SequenceEqual(YamlCodes.Inf0) ||
                        span.SequenceEqual(YamlCodes.Inf1) ||
                        span.SequenceEqual(YamlCodes.Inf2))
                    {
                        value = double.PositiveInfinity;
                        parser.Read();
                        return;
                    }

                    if (span.SequenceEqual(YamlCodes.Nan0) ||
                        span.SequenceEqual(YamlCodes.Nan1) ||
                        span.SequenceEqual(YamlCodes.Nan2))
                    {
                        value = double.NaN;
                        parser.Read();
                        return;
                    }
                    break;
                case 5:
                    if (span.SequenceEqual(YamlCodes.Inf3) ||
                        span.SequenceEqual(YamlCodes.Inf4) ||
                        span.SequenceEqual(YamlCodes.Inf5))
                    {
                        value = double.PositiveInfinity;
                        return;
                    }
                    if (span.SequenceEqual(YamlCodes.NegInf0) ||
                        span.SequenceEqual(YamlCodes.NegInf1) ||
                        span.SequenceEqual(YamlCodes.NegInf2))
                    {
                        value = double.NegativeInfinity;
                        parser.Read();
                        return;
                    }
                    break;
            }
        }
    }
}
