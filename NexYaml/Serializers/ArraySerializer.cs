using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ArraySerializer<T> : YamlSerializer<T?[]>
{
    public override void Write<X>(WriteContext<X> context, T?[] value, DataStyle style)
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

    public override async ValueTask<T?[]?> Read(Scope scope, T?[]? parseResult)
    {
        var sequenceScope = scope.As<SequenceScope>();
        var tasks = new List<Task<T?>>();
        foreach (var element in sequenceScope)
        {
            tasks.Add(element.Read<T>(default).AsTask());
        }
        return (await Task.WhenAll(tasks)).ToArray();
    }
}
