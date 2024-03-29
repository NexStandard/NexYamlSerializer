#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;


public class InterfaceReadOnlyListFormatter<T> : YamlSerializer<IReadOnlyList<T>?>,IYamlFormatter<IReadOnlyList<T>?>
{
    public void IndirectSerialize(ref Utf8YamlEmitter emitter, object value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        Serialize(ref emitter, (List<T>?)value, context,style);
    }
    public void Serialize(ref Utf8YamlEmitter emitter, IReadOnlyList<T>? value, YamlSerializationContext context, DataStyle style = DataStyle.Normal)
    {
        if (value is null)
        {
            emitter.WriteNull();
            return;
        }

        emitter.BeginSequence();
        if (value.Count > 0)
        {
            foreach (var x in value)
            {
                context.Serialize(ref emitter, x);
            }
        }
        emitter.EndSequence();
    }

    public override IReadOnlyList<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
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
            list.Add(value!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ref IYamlStream stream, IReadOnlyList<T>? value, DataStyle style = DataStyle.Normal)
    {
        stream.Emitter.BeginSequence();
        if (value.Count > 0)
        {
            foreach (var x in value)
            {
                stream.Write(x);
            }
        }
        stream.Emitter.EndSequence();
    }
}
