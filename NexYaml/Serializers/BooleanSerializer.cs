using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;
namespace NexYaml.Serializers;

public class BooleanSerializer : YamlSerializer<bool>
{
    public static readonly BooleanSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, bool value, DataStyle style)
    {
        context.WriteScalar(value ? ['t', 'r', 'u', 'e'] : ['f', 'a', 'l', 's', 'e']);
    }

    public override ValueTask<bool> Read(Scope scope, ParseContext parseResult)
    {
        var s = scope.As<XParser.ScalarScope>();
        return new(bool.Parse(s.Value));
    }
}
