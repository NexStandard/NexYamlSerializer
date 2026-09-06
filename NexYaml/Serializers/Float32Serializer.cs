using System.Globalization;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class Float32Serializer : IYamlSerializer<float>
{
    public void Write(Node node, float value, DataStyle style)
    {
        Span<char> span = stackalloc char[32];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        node.WriteScalar(span[..written]);
    }

    public ValueTask<float> Read(Scope scope, float parseResult)
    {
        return new(float.Parse(scope.AsScalar(), CultureInfo.InvariantCulture));
    }
}
