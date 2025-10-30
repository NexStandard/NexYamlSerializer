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
        var scalarScope = scope.As<ScalarScope>();
        // Given the changes above, is this correct, wouldn't this return a null string if the string contains "!!null" ? -Eideren
        if (scalarScope.Value == YamlCodes.Null)
            return default;
        return new(scalarScope.Value);
    }
}
