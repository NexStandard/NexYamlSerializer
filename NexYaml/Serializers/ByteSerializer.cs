using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ByteSerializer : YamlSerializer<byte>
{
    public override void Write<X>(WriteContext<X> context, byte value, DataStyle style)
    {
        Span<char> span = stackalloc char[3];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<byte> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(byte.Parse(scalarScope.Value));
    }
}
