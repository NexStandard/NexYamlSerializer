#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class InterfaceEnumerableFormatter<T> : YamlSerializer<IEnumerable<T>?>
{
    protected override void Write(IYamlWriter stream, IEnumerable<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x, style);
        }

        stream.EndSequence();
    }

    protected override void Read(IYamlReader parser, ref IEnumerable<T>? value)
    {
        var list = new List<T>();
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        while (parser.HasSequence)
        {
            T? val = default;
            parser.Read(ref val);
            list.Add(val!);
        }
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list;
    }
}
