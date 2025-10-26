using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class Int16Serializer : YamlSerializer<short>
{
    public static readonly Int16Serializer Instance = new();
    public override void Write<X>(WriteContext<X> context, short value, DataStyle style)
    {
        Span<char> span = stackalloc char[6];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<short> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(short.Parse(scalarScope.Value, CultureInfo.InvariantCulture));
    }
}
