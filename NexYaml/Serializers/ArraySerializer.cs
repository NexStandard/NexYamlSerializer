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

    public override async ValueTask<T[]?> Read(IYamlReader stream, ParseContext parseResult)
    {
        var tasks = new List<Task<T>>();
        stream.Move(ParseEventType.SequenceStart);
        while (stream.HasSequence)
        {
            tasks.Add(stream.Read<T>(parseResult).AsTask());
        }
        stream.Move(ParseEventType.SequenceEnd);
        return (await Task.WhenAll(tasks)).ToArray();
    }
}
