using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;

public class ArraySerializer<T> : YamlSerializer<T[]>
{
    public override void Write(IYamlWriter stream, T[] value, DataStyle style)
    {
        using (stream.SequenceScope(style))
        {
            foreach (var x in value)
            {
                var val = x;
                stream.Write(val, style);
            }
        }
    }

    public override void Read(IYamlReader stream, ref T[] value, ref ParseResult result)
    {
        var list = new List<T>();

        foreach(var val in stream.ReadAsSequenceOf<T>())
            list.Add(val);

        value = [.. list];
    }
}
