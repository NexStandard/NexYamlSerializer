using System.Globalization;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float32Serializer : IYamlSerializer<float>
{
    public void Write(Node context, float value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public ValueTask<float> Read(Scope scope, float parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(float.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
