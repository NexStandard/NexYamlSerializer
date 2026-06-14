using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeSerializer : IYamlSerializer<DateTime>
{
    public void Write(Node context, DateTime value, DataStyle style)
    {
        Span<char> buf = stackalloc char[32]; // reicht für jedes DateTime-Format
        if (value.TryFormat(buf, out int written, "O")) // ISO 8601, stabil, eindeutig
        {
            context.WriteScalar(buf[..written]);
        }
    }

    public ValueTask<DateTime> Read(Scope scope, DateTime parseResult)
    {
        return new(DateTime.Parse(scope.AsScalar()));
    }
}
