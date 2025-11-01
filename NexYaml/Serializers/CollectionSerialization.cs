using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public static class CollectionSerialization
{
    public static void WriteCollection<X, T, TCol>(WriteContext<X> context, TCol value, DataStyle style, string tag) where X : Node where TCol : ICollection<T?>
    {
        if (value.Count == 0)
        {
            context.WriteEmptySequence(tag);
            return;
        }

        var hasIdentifiable = false;
        foreach (var item in value)
        {
            if (item is IIdentifiable)
            {
                hasIdentifiable = true;
                break;
            }
        }

        if (hasIdentifiable)
        {
            var reservedIds = new HashSet<IIdentifiable>();
            foreach (var element in value)
            {
                if (element is IIdentifiable identifiable
                    && context.Writer.References.Add(identifiable.Id))
                {
                    reservedIds.Add(identifiable);
                }
            }

            var removedIds = new HashSet<IIdentifiable>();
            var resultContext = context.BeginSequence(tag, style);
            foreach (var element in value)
            {
                if (element is IIdentifiable identifiable
                    && reservedIds.Contains(identifiable) &&
                    !removedIds.Contains(identifiable))
                {
                    context.Writer.References.Remove(identifiable.Id);
                    removedIds.Add(identifiable);
                }
                resultContext = resultContext.Write(element, DataStyle.Any);
            }
            resultContext.End(context);
            return;
        }

        var result = context.BeginSequence(tag, style);
        foreach (var x in value)
        {
            result = result.Write(x, DataStyle.Any);
        }
        result.End(context);
    }

    /// <summary>
    /// Append each element inside <paramref name="scope"/> to <paramref name="collection"/>
    /// </summary>
    public static async ValueTask ReadCollection<T, TCol>(Scope scope, TCol collection) where TCol : ICollection<T?>
    {
        var tasks = new List<ValueTask<T?>>();
        foreach (var element in scope.As<SequenceScope>())
        {
            tasks.Add(element.Read<T>());
        }
        foreach( var task in tasks)
        {
            collection.Add(await task);
        }
    }
}
