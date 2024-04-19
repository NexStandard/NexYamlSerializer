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
            var value = default(T);
            context.DeserializeWithAlias<T>(ref parser, ref value);
            list.Add(value!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list.ToArray();
    }

    public override void Serialize(ISerializationWriter stream, T[] value, DataStyle style = DataStyle.Any)
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
}
