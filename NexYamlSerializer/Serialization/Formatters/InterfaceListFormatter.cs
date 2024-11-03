#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceListFormatter<T> : YamlSerializer<IList<T>>
{
    public override void Write(IYamlWriter stream, IList<T> value, DataStyle style)
    {
        stream.WriteSequence(style,() =>
        {
            foreach (var x in value)
            {
                stream.Write(x, style);
            }
        });
    }

    public override void Read(IYamlReader stream, ref IList<T> value)
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
