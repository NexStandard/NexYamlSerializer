using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : YamlSerializer<TimeSpan>
{
    public override void Write<X>(WriteContext<X> context, TimeSpan value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public override ValueTask<TimeSpan> Read(Scope scope, TimeSpan parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(TimeSpan.Parse(scalarScope.Value));
    }
}
