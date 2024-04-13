#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class ArrayFormatter<T> : YamlSerializer<T[]?>,IYamlFormatter<T[]?>
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

    public override void Serialize(ref ISerializationWriter stream, T[]? value, DataStyle style = DataStyle.Normal)
    {
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if (typeof(T).IsValueType)
            {

            }
            var contentStyle = DataStyle.Any;
            if (style == DataStyle.Compact)
            {
                contentStyle = DataStyle.Compact;
            }
            stream.Emitter.BeginSequence(style);
            foreach (var x in value)
            {
                var val = x;
                stream.Serialize(val, contentStyle);
            }
            stream.Emitter.EndSequence();
        }
    } 
    public void Serialize(ISerializationWriter stream, T[]? value, DataStyle style = DataStyle.Normal)
    {
        if (value == null)
        {
            stream.WriteNull();
        }
        else
        {
            if (typeof(T).IsValueType)
            {

            }
            var contentStyle = DataStyle.Any;
            if (style == DataStyle.Compact)
            {
                contentStyle = DataStyle.Compact;
            }
            stream.Emitter.BeginSequence(style);
            foreach (var x in value)
            {
                var val = x;
                stream.Write(ref val, contentStyle);
            }
            stream.Emitter.EndSequence();
        }
    }
}
