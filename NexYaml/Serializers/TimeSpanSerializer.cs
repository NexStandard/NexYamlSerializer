using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class TimeSpanSerializer : IYamlSerializer<TimeSpan>
{
    public void Write(Node context, TimeSpan value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public ValueTask<TimeSpan> Read(Scope scope, TimeSpan parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(TimeSpan.Parse(scalarScope.Value));
    }
}
