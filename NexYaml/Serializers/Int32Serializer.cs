using System.Globalization;
using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
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
        return new(int.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
