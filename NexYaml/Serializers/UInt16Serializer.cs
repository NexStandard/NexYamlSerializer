using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt16Serializer : YamlSerializer<ushort>
{
    public static readonly UInt16Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, ushort value, DataStyle style)
    {
        Span<char> span = stackalloc char[5];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<ushort> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && ushort.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move(ParseEventType.Scalar);
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(ushort), span, stream.CurrentMarker);
    }
    public override ValueTask<ushort> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(ushort.Parse(scalarScope.Value));
    }
}
