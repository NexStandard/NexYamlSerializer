#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class ArrayFormatter<T> : YamlSerializer<T[]>
{
    protected override void Write(ISerializationWriter stream, T[] value, DataStyle style)
    {
        var contentStyle = DataStyle.Any;
        if (style == DataStyle.Compact)
        {
            contentStyle = DataStyle.Compact;
        }
        stream.BeginSequence(style);
        foreach (var x in value)
        {
            var val = x;
            stream.Write(val, contentStyle);
        }
        stream.EndSequence();
    }

    protected override void Read(YamlParser parser, YamlDeserializationContext context, ref T[] value)
    {
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (parser.HasSequence)
        {
            var val = default(T);
            context.DeserializeWithAlias(ref parser, ref val);
            list.Add(val!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        value = [.. list];
    }
}
