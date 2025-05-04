using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
namespace NexYaml.Serializers;

public class BooleanSerializer : YamlSerializer<bool>
{
    public static readonly BooleanSerializer Instance = new();

    public override WriteContext Write(IYamlWriter stream, bool value, DataStyle style, in WriteContext context)
    {
        return context.Write(value, style);
    }

    public override void Read(IYamlReader stream, ref bool value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsSpan(out var span))
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
        stream.Move();
    }
}
