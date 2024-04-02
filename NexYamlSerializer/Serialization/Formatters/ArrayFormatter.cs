#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class ArrayFormatter<T> : YamlSerializer<T[]?>,IYamlFormatter<T[]?>
{
    public void Serialize(ref Utf8YamlEmitter emitter, T[]? value, YamlSerializationContext context, DataStyle style)
    {
        if(style is DataStyle.Any)
        {
            style = DataStyle.Normal;
        }
        if (value is null)
        {
            emitter.WriteNull();
            return;
        }

        emitter.BeginSequence(style);
        foreach (var x in value)
        {
            context.Serialize(ref emitter, x);
        }
        emitter.EndSequence();
    }

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

    public override void Serialize(ref IYamlStream stream, T[]? value, DataStyle style = DataStyle.Normal)
    {
        stream.Write(ref value);
    }
}
