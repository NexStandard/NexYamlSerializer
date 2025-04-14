using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class InterfaceLisSerializer<T> : YamlSerializer<IList<T>>
{
    public override void Write(IYamlWriter stream, IList<T> value, DataStyle style)
    {
        stream.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Write(x, style);
        }
        stream.EndSequence();
    }

    public override void Read(IYamlReader stream, ref IList<T> value, ref ParseResult result)
    {
        var list = new List<T>();
        foreach (var val in stream.ReadAsSequenceOf<T>())
            list.Add(val);
        value = list;
    }
}
