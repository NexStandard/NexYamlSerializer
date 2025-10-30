using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int16Serializer : IYamlSerializer<short>
{
    public void Write<X>(WriteContext<X> context, short value, DataStyle style) where X : Node
    {
        Span<char> span = stackalloc char[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<short> Read(Scope scope, short parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(short.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
