using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class InterfaceReadOnlyCollectionSerializer<T> : YamlSerializer<IReadOnlyCollection<T>?>
{
    public override void Write(IYamlWriter stream, IReadOnlyCollection<T>? value, DataStyle style)
    {
        stream.WriteSequence(style, () =>
        {
            foreach (var x in value)
            {
                stream.Write(x, style);
            }
        });
    }

    public override void Read(IYamlReader stream, ref IReadOnlyCollection<T>? value, ref ParseResult result)
    {
        var list = new List<T>();
        stream.ReadSequence(() =>
        {
            var val = default(T);
            stream.Read(ref val);
            list.Add(val!);
        });
        value = list!;
    }
}
