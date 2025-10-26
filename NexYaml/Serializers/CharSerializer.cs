using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class CharSerializer : YamlSerializer<char>
{
    public static readonly CharSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, char value, DataStyle style)
    {
        context.WriteScalar(['\'', value, '\'']);
    }

    public override ValueTask<char> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(char.Parse(scalarScope.Value));
    }
}
