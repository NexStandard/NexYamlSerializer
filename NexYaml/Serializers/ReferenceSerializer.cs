using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;
public class ReferenceSerializer<T> : YamlSerializer<T>
    where T : IIdentifiable
{
    private const string refPrefix = "!!ref ";
    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out var reference))
        {
            if (reference == null)
                value = default;
            var id = reference.Substring(refPrefix.Length);
            result = new ParseResult()
            {
                Reference = Guid.Parse(id),
                IsReference = true
            };
        }
    }

    public override void Write<X>(WriteContext<X> context, T value, DataStyle style)
    {
        context.WriteScalar(("!!ref " + value.Id).AsSpan());
    }
}
