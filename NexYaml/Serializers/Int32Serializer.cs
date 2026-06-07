using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int32Serializer : IYamlSerializer<int>
{
    public void Write(Node context, int value, DataStyle style)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<int> Read(Scope scope, int parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(int.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
