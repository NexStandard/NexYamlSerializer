#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyCollectionFormatter<T> : YamlSerializer<IReadOnlyCollection<T>?>
{
    public override void Serialize(ISerializationWriter stream, IReadOnlyCollection<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x);
        }

        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref IReadOnlyCollection<T>? value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            var val = default(T);
            context.DeserializeWithAlias(ref parser, ref val);
            list.Add(val!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list!;
    }
}
