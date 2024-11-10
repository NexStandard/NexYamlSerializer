using NexVYaml;
using NexVYaml.Parser;
using Stride.Core;

namespace NexYamlSerializer.Serialization.Formatters;
public class ReferenceFormatter<T> : YamlSerializer<T>
    where T : IIdentifiable
{
    private const string refPrefix = "!!ref ";
    public override void Read(IYamlReader stream, ref T value)
    {
        if (stream.TryGetScalarAsString(out var reference))
        {
            if (reference == null)
                value = default;
            var id = reference.Substring(refPrefix.Length);

        }
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        stream.Write("!!ref " + value.Id);
    }
}
