using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Globalization;

namespace NexYaml.Serializers;

public class Float64Serializer : YamlSerializer<double>
{
    public static readonly Float64Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, double value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override void Read(IYamlReader stream, ref double value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (double.TryParse(span, CultureInfo.InvariantCulture, out value))
            {
                stream.Move();
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
                        stream.Move();
                        return;
                    }

                    if (span.SequenceEqual(YamlCodes.Nan0) ||
                        span.SequenceEqual(YamlCodes.Nan1) ||
                        span.SequenceEqual(YamlCodes.Nan2))
                    {
                        value = double.NaN;
                        stream.Move();
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
                        stream.Move();
                        return;
                    }
                    break;
            }
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(double), text, stream.CurrentMarker);
        }
    }
}
