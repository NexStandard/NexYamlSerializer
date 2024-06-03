#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class InterfaceEnumerableFormatter<T> : YamlSerializer<IEnumerable<T>?>
{
    protected override void Write(ISerializationWriter stream, IEnumerable<T>? value, DataStyle style)
    {
        stream.BeginSequence(style);

        foreach (var x in value!)
        {
            stream.Write(x, style);
        }

        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref IEnumerable<T>? value)
    {
        var list = new List<T>();
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            T? val = default;
            context.DeserializeWithAlias(ref parser, ref val);
            list.Add(val!);
        }
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list;
    }
}
