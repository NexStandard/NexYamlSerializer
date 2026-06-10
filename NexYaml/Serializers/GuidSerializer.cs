using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class GuidSerializer : IYamlSerializer<Guid>
{
    public void Write(Node context, Guid value, DataStyle style)
    {
        Span<char> buffer = stackalloc char[36]; // or 32 if you want "N" format

        if (value.TryFormat(buffer, out int written, "D"))
        {
            context.WriteScalar(buffer);
        }
    }

    public ValueTask<Guid> Read(Scope scope, Guid parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(Guid.Parse(scalarScope.Value));
    }
}
