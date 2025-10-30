using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : IYamlSerializer<string?>
{
    public void Write<X>(WriteContext<X> context, string? value, DataStyle style) where X : Node
    {
        // Is this correct ? -Eideren
        if (value is null)
            context.WriteScalar(YamlCodes.Null);
        else
            context.WriteScalar(context.Writer.FormatString(context, value, style));
    }

    public ValueTask<string?> Read(Scope scope, string? parseResult)
    {
        // Given the changes above, what should we do here ? -Eideren
        return new(scope.As<ScalarScope>().Value);
    }
}
