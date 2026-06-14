using System.Globalization;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int64Serializer : IYamlSerializer<long>
{
    public void Write(Node context, long value, DataStyle style)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<long> Read(Scope scope, long parseResult)
    {
        return new(long.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
