using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : YamlSerializer<char>
{
    public static readonly CharSerializer Instance = new();

    public override void Write(IYamlWriter stream, char value, DataStyle style)
    {
        stream.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref char value, ref ParseResult parseResult)
    {
        if (stream.TryGetScalarAsString(out var result))
        {
            if (result is not null && result.Length == 1)
            {
                if (result.Length == 1)
                {
                    value = result[0];
                }
            }
        }

        stream.ReadWithVerify(ParseEventType.Scalar);
    }
}
