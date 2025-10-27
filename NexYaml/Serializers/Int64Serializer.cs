using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int64Serializer : YamlSerializer<long>
{
    public static readonly Int64Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, long value, DataStyle style)
    {
        Span<char> span = stackalloc char[20];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<long> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(long.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
