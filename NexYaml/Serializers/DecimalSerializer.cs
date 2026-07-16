using System.Globalization;

using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DecimalSerializer : IYamlSerializer<decimal>
{
    public void Write(Node context, decimal value, DataStyle style)
    {
        Span<char> span = stackalloc char[64];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<decimal> Read(Scope scope, decimal parseResult)
    {
        return new(decimal.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
