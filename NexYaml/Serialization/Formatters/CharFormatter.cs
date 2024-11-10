using NexVYaml.Parser;
using Stride.Core;

namespace NexYaml.Serialization.Formatters;

public class CharFormatter : YamlSerializer<char>
{
    public static readonly CharFormatter Instance = new();

    public override void Write(IYamlWriter stream, char value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader parser, ref char value)
    {
        if (parser.TryGetScalarAsString(out var result))
        {
            if (result is not null && result.Length == 1)
            {
                if (result.Length == 1)
                {
                    value = result[0];
                }
            }
        }

        parser.ReadWithVerify(ParseEventType.Scalar);
    }
}
