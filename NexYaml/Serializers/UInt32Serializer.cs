using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class UInt32Serializer : YamlSerializer<uint>
{
    public static readonly UInt32Serializer Instance = new();

    public override void Write<X>(WriteContext<X> context, uint value, DataStyle style)
    {
        Span<char> span = stackalloc char[10];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        context.WriteScalar(span[..written]);
    }

    public override ValueTask<uint> Read(IYamlReader stream, ParseContext parseResult)
    {
        if (stream.TryGetScalarAsString(out var span) && uint.TryParse(span, CultureInfo.InvariantCulture, out var value))
        {
            stream.Move(ParseEventType.Scalar);
            return new(value);
        }
        stream.SkipRead();
        throw YamlException.ThrowExpectedTypeParseException(typeof(uint), span, stream.CurrentMarker);
    }
    public override ValueTask<uint> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(uint.Parse(scalarScope.Value));
    }
}
