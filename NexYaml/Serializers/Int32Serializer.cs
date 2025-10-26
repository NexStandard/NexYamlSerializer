using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int32Serializer : YamlSerializer<int>
{
    public static readonly Int32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, int value, DataStyle style)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<int> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(int.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
