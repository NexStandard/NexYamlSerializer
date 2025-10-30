using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : YamlSerializer<DateTimeOffset>
{
    public override void Write<X>(WriteContext<X> context, DateTimeOffset value, DataStyle style)
    {
        context.WriteType(value.ToString(), style);
    }

    public override ValueTask<DateTimeOffset> Read(Scope scope, DateTimeOffset parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTimeOffset.Parse(scalarScope.Value));
    }
}
