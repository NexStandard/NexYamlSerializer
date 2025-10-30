using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class SByteSerializer : YamlSerializer<sbyte>
{
    public override void Write<X>(WriteContext<X> context, sbyte value, DataStyle style)
    {
        Span<char> span = stackalloc char[4];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<sbyte> Read(Scope scope, sbyte parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(sbyte.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
