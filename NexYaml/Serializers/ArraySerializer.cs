using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ArraySerializer<T> : IYamlSerializer<T?[]>
{
    public void Write<X>(WriteContext<X> context, T?[] value, DataStyle style) where X : Node
    {
        if (value.Length == 0)
        {
            context.WriteEmptySequence("!Array");
        }
        var result = context.BeginSequence("!Array", style);

        foreach (var element in value)
        {
            result = result.Write(element, style);
        }
        result.End(context);
    }

    public async ValueTask<T?[]> Read(Scope scope, T?[]? parseResult = null)
    {
        var sequenceScope = scope.As<SequenceScope>();
        var list = new List<T?>();
        var tasks = new List<ValueTask<T?>>();
        foreach (var element in sequenceScope)
        {
            tasks.Add(element.Read<T>());
        }
        foreach (var task in tasks)
        {
            list.Add(await task);
        }
        return list.ToArray();
    }
}
