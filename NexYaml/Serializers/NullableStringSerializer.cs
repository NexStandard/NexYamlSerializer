
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class NullableStringSerializer : IYamlSerializer<string?>
{
    public void Write(Node node, string? value, DataStyle style)
    {
        // Should be taken care of in the caller's scope, which would be Writer.WriteType
        System.Diagnostics.Debug.Assert(value is not null);
        node.WriteScalar(node.Writer.FormatString(node, value, style));
    }

    public ValueTask<string?> Read(Scope scope, string? parseResult)
    {
        return new(scope.AsScalar().ToString());
    }
}
