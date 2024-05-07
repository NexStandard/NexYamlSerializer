#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceListFormatter<T> : YamlSerializer<IList<T>>
{
    public override void Serialize(ISerializationWriter stream, IList<T> value, DataStyle style)
    {
        stream.BeginSequence(style);
        if (value.Count > 0)
        {
            foreach (var x in value)
            {
                stream.Write(x);
            }
        }
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref IList<T> value)
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
