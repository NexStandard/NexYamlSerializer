using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : IYamlSerializer<string?>
{
    public void Write<X>(WriteContext<X> context, string? value, DataStyle style) where X : Node
    {
        // Should be taken care of in the caller's scope, which would be Writer.WriteType
        System.Diagnostics.Debug.Assert(value is not null);
        context.WriteScalar(context.Writer.FormatString(context, value, style));
    }

    public ValueTask<string?> Read(Scope scope, string? parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        // Should be taken care of in the caller's scope, which would be ScopeExtensions.Read
        System.Diagnostics.Debug.Assert(scalarScope.Value != YamlCodes.Null);
        return new(scalarScope.Value);
    }
}
