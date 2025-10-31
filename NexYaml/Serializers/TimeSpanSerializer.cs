using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : IYamlSerializer<TimeSpan>
{
    public void Write<X>(WriteContext<X> context, TimeSpan value, DataStyle style) where X : Node
    {
        context.WriteString(value.ToString());
    }

    public ValueTask<TimeSpan> Read(Scope scope, TimeSpan parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(TimeSpan.Parse(scalarScope.Value));
    }
}
