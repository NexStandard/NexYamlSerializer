
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class DateTimeOffsetSerializer : IYamlSerializer<DateTimeOffset>
{
    public void Write(Node context, DateTimeOffset value, DataStyle style)
    {
        Span<char> buf = stackalloc char[33];
        value.TryFormat(buf, out int written, "O");

        context.WriteScalar(buf[..written]);
    }

    public ValueTask<DateTimeOffset> Read(Scope scope, DateTimeOffset parseResult)
    {
        return new(DateTimeOffset.Parse(scope.AsScalar()));
    }
}
