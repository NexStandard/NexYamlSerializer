using System.Globalization;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml.Serializers;

public class GuidSerializer : YamlSerializer<Guid>
{
    public static readonly GuidSerializer Instance = new();

    public override void Write<X>(WriteContext<X> context, Guid value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public override ValueTask<Guid> Read(Scope scope, ParseContext parseResult)
    {
        var scalarScope = scope.As<XParser.ScalarScope>();
        return new(Guid.Parse(scalarScope.Value));
    }
}
