using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeSerializer : IYamlSerializer<DateTime>
{
    public void Write<X>(WriteContext<X> context, DateTime value, DataStyle style) where X : Node
    {
        context.WriteType(value.ToString(), style);
    }

    public ValueTask<DateTime> Read(Scope scope, DateTime parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTime.Parse(scalarScope.Value));
    }
}
