#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml.Serialization;

public class InterfaceEnumerableFormatter<T> : YamlSerializer<IEnumerable<T>?>
{
    public override IEnumerable<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
        }

        var list = new List<T>();
        parser.ReadWithVerify(ParseEventType.SequenceStart);

        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            T? value = default;
            context.DeserializeWithAlias<T>(ref parser, ref value);
            list.Add(value!);
        }
        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ISerializationWriter stream, IEnumerable<T> value, DataStyle style = DataStyle.Normal)
    {
        stream.BeginSequence(style);
        if (value.Any())
        {
            foreach (var x in value)
            {
                stream.Serialize(x, style);
            }
        }
        stream.EndSequence();
    }
}
