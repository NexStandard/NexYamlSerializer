#nullable enable
using System.Collections.Generic;
using System.Linq;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;

public class InterfaceEnumerableFormatter<T> : YamlSerializer<IEnumerable<T>?>,IYamlFormatter<IEnumerable<T>?>
{
    public override IEnumerable<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
        }

        List<T> list = new List<T>();
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            list.Add(context.DeserializeWithAlias<T>(ref parser)!);
        }
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ref ISerializationWriter stream, IEnumerable<T>? value, DataStyle style = DataStyle.Normal)
    {
        stream.Emitter.BeginSequence();
        if (value.Any())
        {
            foreach (var x in value)
            {
                stream.Write(x);
            }
        }
        stream.Emitter.EndSequence();
    }
}
