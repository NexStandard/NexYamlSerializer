#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class ArrayFormatter<T> : YamlSerializer<T[]?>
{
    public override T[]? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }

        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            var value = context.DeserializeWithAlias<T>(ref parser);
            list.Add(value);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list.ToArray();
    }

    public override void Serialize(ref ISerializationWriter stream, T[] value, DataStyle style = DataStyle.Normal)
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
            stream.Serialize(val, contentStyle);
        }
        stream.EndSequence();

    }
    public void Serialize(ISerializationWriter stream, T[]? value, DataStyle style = DataStyle.Normal)
    {
        var contentStyle = DataStyle.Any;
        if (style == DataStyle.Compact)
        {
            contentStyle = DataStyle.Compact;
        }
        stream.BeginSequence(style);
        foreach (var item in value)
        {
            stream.Serialize(item, contentStyle);
        }
        stream.EndSequence();
    }
}
