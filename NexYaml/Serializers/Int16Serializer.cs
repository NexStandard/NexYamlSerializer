using System.Globalization;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int16Serializer : IYamlSerializer<short>
{
    public void Write(Node context, short value, DataStyle style)
    {
        Span<char> span = stackalloc char[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<short> Read(Scope scope, short parseResult)
    {
        return new(short.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
