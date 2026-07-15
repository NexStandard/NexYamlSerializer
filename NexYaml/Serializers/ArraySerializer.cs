using NexYaml.Core.Serialization.Nodes;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ArraySerializer<T> : IYamlSerializer<T?[]>
{
    public void Write(Node context, T?[] value, DataStyle style)
    {
        CollectionSerialization.WriteCollection<T, T?[]>(context, value, style, "!Array");
    }

    public async ValueTask<T?[]> Read(Scope scope, T?[]? parseResult = null)
    {
        var tasks = new List<ValueTask<T?>>();
        foreach (var element in scope.AsSequence())
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
