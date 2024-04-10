#nullable enable
using System.Collections.Generic;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Audio;
using Stride.Core;

namespace NexVYaml.Serialization;

public class InterfaceListFormatter<T> : YamlSerializer<IList<T>>,IYamlFormatter<IList<T>?>
{
    public override IList<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
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
        return list!;
    }

    public override void Serialize(ref ISerializationWriter stream, IList<T> value, DataStyle style = DataStyle.Normal)
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
