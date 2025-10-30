using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt64Serializer : IYamlSerializer<ulong>
{
    public void Write<X>(WriteContext<X> context, ulong value, DataStyle style) where X : Node
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<ulong> Read(Scope scope, ulong parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(ulong.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
