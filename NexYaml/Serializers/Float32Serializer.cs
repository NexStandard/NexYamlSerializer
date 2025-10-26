using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float32Serializer : YamlSerializer<float>
{
    public static readonly Float32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, float value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<float> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(float.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
