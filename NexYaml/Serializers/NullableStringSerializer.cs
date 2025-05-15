using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : YamlSerializer<string?>
{
    public static readonly NullableStringSerializer Instance = new();
    public override void Write<X>(WriteContext<X> context, string? value, DataStyle style)
    {
        if (value is null)
        {
            context.WriteScalar(YamlCodes.Null);
            return;
        }
        var result = EmitStringAnalyzer.Analyze(value);
        var scalarStyle = result.SuggestScalarStyle();
        if (scalarStyle is ScalarStyle.Plain or ScalarStyle.Any)
        {
            context.WriteScalar(value);
            return;
        }
        else if (ScalarStyle.Folded == scalarStyle)
        {
            throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
        }
        else if (ScalarStyle.SingleQuoted == scalarStyle)
        {
            throw new InvalidOperationException("Single Quote is reserved for char");
        }
        else if (ScalarStyle.DoubleQuoted == scalarStyle)
        {
            context.WriteScalar("\"" + value + "\"");
            return;
        }
        else if (ScalarStyle.Literal == scalarStyle)
        {
            var indentCharCount = (context.Indent + 1) * context.Indent;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            context.WriteScalar(scalarStringBuilt.ToString());
            return;
        }
        // TODO is this reachable?
        throw new YamlException("Couldnt get ScalarStyle");
    }

    public override void Read(IYamlReader stream, ref string? value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out value))
        {
            stream.Move(ParseEventType.Scalar);
            return;
        }
        value = null;
    }
}

