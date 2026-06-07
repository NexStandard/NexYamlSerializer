using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt32Serializer : IYamlSerializer<uint>
{
    public void Write(Node context, uint value, DataStyle style)
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
