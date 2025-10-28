using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt16Serializer : YamlSerializer<ushort>
{
    public override void Write<X>(WriteContext<X> context, ushort value, DataStyle style)
    {
        Span<char> span = stackalloc char[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<ushort> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(ushort.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
