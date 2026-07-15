using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
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
        return new(TimeSpan.Parse(scope.AsScalar()));
    }
}
