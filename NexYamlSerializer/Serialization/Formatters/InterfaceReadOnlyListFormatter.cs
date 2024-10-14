#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyListFormatter<T> : YamlSerializer<IReadOnlyList<T>?>
{
    protected override void Write(IYamlWriter stream, IReadOnlyList<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x, style);
        }

        stream.EndSequence();
    }

    protected override void Read(IYamlReader parser, ref IReadOnlyList<T>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (parser.HasSequence)
        {
            var val = default(T);
            parser.Read(ref val);
            list.Add(val!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list!;
    }
}
