using NexYaml.Core;
using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : IYamlSerializer<string?>
{
    public void Write(Node context, string? value, DataStyle style)
    {
        // Should be taken care of in the caller's scope, which would be Writer.WriteType
        System.Diagnostics.Debug.Assert(value is not null);
        context.WriteScalar(context.Writer.FormatString(context, value, style));
    }

    public ValueTask<string?> Read(Scope scope, string? parseResult)
    {
        return new(scope.AsScalar().ToString());
    }
}
