using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt32Serializer : IYamlSerializer<uint>
{
    public void Write<X>(WriteContext<X> context, uint value, DataStyle style) where X : Node
    {
        Span<char> span = stackalloc char[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<uint> Read(Scope scope, uint parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(uint.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
