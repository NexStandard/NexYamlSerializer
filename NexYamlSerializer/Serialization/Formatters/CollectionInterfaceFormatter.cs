#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class CollectionInterfaceFormatter<T> : YamlSerializer<ICollection<T>?>
{
    protected override void Write(IYamlWriter stream, ICollection<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x, style);
        }

        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, ref ICollection<T>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T?>();
        while (parser.HasSequence)
        {
            var val = default(T);
            parser.DeserializeWithAlias(ref parser, ref value);
            list.Add(val);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list!;
    }
}
