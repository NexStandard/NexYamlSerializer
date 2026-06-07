using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeSerializer : IYamlSerializer<DateTime>
{
    public void Write(Node context, DateTime value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public ValueTask<DateTime> Read(Scope scope, DateTime parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTime.Parse(scalarScope.Value));
    }
}
