using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : IYamlSerializer<DateTimeOffset>
{
    public void Write<X>(WriteContext<X> context, DateTimeOffset value, DataStyle style) where X : Node
    {
        context.WriteType(value.ToString(), style);
    }

    public ValueTask<DateTimeOffset> Read(Scope scope, DateTimeOffset parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTimeOffset.Parse(scalarScope.Value));
    }
}
