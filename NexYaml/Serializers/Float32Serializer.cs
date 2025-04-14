using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Globalization;

namespace NexYaml.Serializers;

public class Float32Serializer : YamlSerializer<float>
{
    public static readonly Float32Serializer Instance = new();

    public override void Write(IYamlWriter stream, float value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref float value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
        {
            if (float.TryParse(span, CultureInfo.InvariantCulture, out value))
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
                        value = float.PositiveInfinity;
                        stream.Move();
                        return;
                    }

                    if (span.SequenceEqual(YamlCodes.Nan0) ||
                        span.SequenceEqual(YamlCodes.Nan1) ||
                        span.SequenceEqual(YamlCodes.Nan2))
                    {
                        value = float.NaN;
                        stream.Move();
                        return;
                    }
                    break;
                case 5:
                    if (span.SequenceEqual(YamlCodes.Inf3) ||
                        span.SequenceEqual(YamlCodes.Inf4) ||
                        span.SequenceEqual(YamlCodes.Inf5))
                    {
                        value = float.PositiveInfinity;
                        stream.Move();
                        return;
                    }
                    if (span.SequenceEqual(YamlCodes.NegInf0) ||
                        span.SequenceEqual(YamlCodes.NegInf1) ||
                        span.SequenceEqual(YamlCodes.NegInf2))
                    {
                        value = float.NegativeInfinity;
                        stream.Move();
                        return;
                    }
                    break;
            }
            stream.TryGetScalarAsString(out var text);
            YamlException.ThrowExpectedTypeParseException(typeof(float), text, stream.CurrentMarker);
        }
    }
}
