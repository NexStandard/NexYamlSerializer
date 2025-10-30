using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeSerializer : YamlSerializer<DateTime>
{
    public override void Write<X>(WriteContext<X> context, DateTime value, DataStyle style)
    {
        context.WriteType(value.ToString(), style);
    }

    public override ValueTask<DateTime> Read(Scope scope, DateTime parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTime.Parse(scalarScope.Value));
    }
}
