using System.Globalization;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt64Serializer : IYamlSerializer<ulong>
{
    public void Write(Node node, ulong value, DataStyle style)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        node.WriteScalar(span[..written]);
    }

    public ValueTask<ulong> Read(Scope scope, ulong parseResult)
    {
        return new(ulong.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
