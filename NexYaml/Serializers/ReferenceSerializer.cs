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

    public override WriteContext Write(IYamlWriter stream, T value, DataStyle style, in WriteContext context)
    {
        return context.Write(("!!ref " + value.Id).AsSpan());
    }
}
