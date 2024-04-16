#nullable enable
using NexVYaml.Parser;
using NexYamlSerializer;
using Stride.Core;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class InterfaceReadOnlyListFormatter<T> : YamlSerializer<IReadOnlyList<T>?>
{
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

    public override void Serialize(ISerializationWriter stream, IReadOnlyList<T>? value, DataStyle style = DataStyle.Normal)
    {
        stream.BeginSequence(style);
        YamlSerializer<T>? y = null;
        if (typeof(T).IsValueType || typeof(T) == typeof(string))
            y = stream.SerializeContext.Resolver.GetFormatter<T>();
        if(y is not null)
        {
            foreach (var x in value)
            {
                y.Serialize(stream, x,style);
            }
        }
        else
        {
            foreach (var x in value)
            {
                stream.Serialize(x, style);
            }
        }

        stream.EndSequence();
    }
}
