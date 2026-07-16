using System.Globalization;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class SByteSerializer : IYamlSerializer<sbyte>
{
    public void Write(Node context, sbyte value, DataStyle style)
    {
        Span<char> span = stackalloc char[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<sbyte> Read(Scope scope, sbyte parseResult)
    {
        return new(sbyte.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
