using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float64Serializer : IYamlSerializer<double>
{
    public void Write<X>(WriteContext<X> context, double value, DataStyle style) where X : Node
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<double> Read(Scope scope, double parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(double.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
