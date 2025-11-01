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
        var tasks = new List<ValueTask<T?>>();
        foreach (var element in scope.As<SequenceScope>())
        {
            tasks.Add(element.Read<T>());
        }

        if (parseResult != null)
        {
            for (var i = 0; i < tasks.Count; i++)
            {
                var result = await tasks[i];
                if (i < parseResult.Length)
                    parseResult[i] = result;
            }

            return parseResult;
        }
        else
        {
            var list = new List<T?>();
            foreach (var task in tasks)
            {
                list.Add(await task);
            }
            return list.ToArray();
        }
    }
}
