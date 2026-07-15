using System.Globalization;
using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt16Serializer : IYamlSerializer<ushort>
{
    public void Write(Node context, ushort value, DataStyle style)
    {
        Span<char> span = stackalloc char[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<ushort> Read(Scope scope, ushort parseResult)
    {
        return new(ushort.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
