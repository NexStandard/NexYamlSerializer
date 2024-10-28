#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyListFormatter<T> : YamlSerializer<IReadOnlyList<T>?>
{
    protected override void Write(IYamlWriter stream, IReadOnlyList<T>? value, DataStyle style)
    {
        stream.WriteSequence(style, () =>
        {
            foreach (var x in value)
            {
                stream.Write(x, style);
            }
        });
    }

    protected override void Read(IYamlReader stream, ref IReadOnlyList<T>? value)
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
