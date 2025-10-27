using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float64Serializer : YamlSerializer<double>
{
    public static readonly Float64Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, double value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<double> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(double.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
