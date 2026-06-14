using System.Globalization;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ByteSerializer : IYamlSerializer<byte>
{
    public void Write(Node context, byte value, DataStyle style)
    {
        Span<char> span = stackalloc char[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<byte> Read(Scope scope, byte parseResult)
    {
        return new(byte.Parse(scope.AsScalar()));
    }
}
