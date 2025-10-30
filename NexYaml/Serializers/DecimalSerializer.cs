using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DecimalSerializer : YamlSerializer<decimal>
{
    public override void Write<X>(WriteContext<X> context, decimal value, DataStyle style)
    {
        Span<char> span = stackalloc char[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<decimal> Read(Scope scope, decimal parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(decimal.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
