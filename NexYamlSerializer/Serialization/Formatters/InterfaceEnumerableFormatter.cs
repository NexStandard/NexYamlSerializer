using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceEnumerableFormatter<T> : YamlSerializer<IEnumerable<T>?>
{
    public override void Write(IYamlWriter stream, IEnumerable<T>? value, DataStyle style)
    {
        stream.WriteSequence(style, () =>
        {
            foreach (var x in value!)
            {
                stream.Write(x, style);
            }
        });
    }

    public override void Read(IYamlReader stream, ref IEnumerable<T>? value)
    {
        var list = new List<T>();

        stream.ReadSequence(() =>
        {
            T? val = default;
            stream.Read(ref val);
            list.Add(val!);
        });
        value = list;
    }
}
