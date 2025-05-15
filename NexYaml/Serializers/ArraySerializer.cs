using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ArraySerializer<T> : YamlSerializer<T[]>
{
    public override void Write<X>(WriteContext<X> context, T[] value, DataStyle style)
    {
        if (value.Length == 0)
        {
            context.WriteEmptySequence("!Array");
        }
        var result = context.BeginSequence("!Array", style);

        foreach (var x in value)
        {
            var val = x;
            result = result.Write(x, style);
        }
        result.End(context);
    }

    public override void Read(IYamlReader stream, ref T[] value, ref ParseResult result)
    {
        var list = new List<T>();

        foreach(var val in stream.ReadAsSequenceOf<T>())
            list.Add(val);

        value = [.. list];
    }
}
