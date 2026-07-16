using System.Globalization;
using NexYaml.Parser.Scopes;
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
        return new(uint.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
