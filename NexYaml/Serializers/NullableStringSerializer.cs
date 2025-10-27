using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : YamlSerializer<string>
{
    public static readonly NullableStringSerializer Instance = new();
    public override void Write<X>(WriteContext<X> context, string value, DataStyle style)
    {
        context.WriteScalar(context.Writer.FormatString(context, value, style));
    }

    public override ValueTask<string?> Read(Scope scope, ParseContext parseResult)
    {
        return new(scope.As<ScalarScope>().Value);
    }
}
