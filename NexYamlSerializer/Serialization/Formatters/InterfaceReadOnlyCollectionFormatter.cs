#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyCollectionFormatter<T> : YamlSerializer<IReadOnlyCollection<T>?>
{
    protected override void Write(IYamlWriter stream, IReadOnlyCollection<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x, style);
        }

        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, ref IReadOnlyCollection<T>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (parser.HasSequence)
        {
            var val = default(T);
            parser.DeserializeWithAlias(ref parser, ref val);
            list.Add(val!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list!;
    }
}
