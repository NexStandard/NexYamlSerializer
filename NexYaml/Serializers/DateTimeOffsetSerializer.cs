using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : IYamlSerializer<DateTimeOffset>
{
    public void Write(Node context, DateTimeOffset value, DataStyle style)
    {
        context.WriteString(value.ToString());
    }

    public ValueTask<DateTimeOffset> Read(Scope scope, DateTimeOffset parseResult)
    {
        var scalarScope = scope.As<ScalarScope>();
        return new(DateTimeOffset.Parse(scalarScope.Value));
    }
}
