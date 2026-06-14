using System.Globalization;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float64Serializer : IYamlSerializer<double>
{
    public void Write(Node context, double value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<double> Read(Scope scope, double parseResult)
    {
        return new(double.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
