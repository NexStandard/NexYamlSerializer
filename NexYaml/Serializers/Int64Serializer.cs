using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int64Serializer : IYamlSerializer<long>
{
    public void Write<X>(WriteContext<X> context, long value, DataStyle style) where X : Node
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<long> Read(Scope scope, long parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(long.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
