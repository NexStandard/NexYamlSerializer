using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ListSerializer<T> : IYamlSerializer<List<T?>>
{
    public string? CustomTag { get; init; }
    public void Write<X>(WriteContext<X> context, List<T?> value, DataStyle style) where X : Node
    {
        bool hasIdentifiable = false;
        if (value.Count == 0)
        {
            context.WriteEmptySequence(CustomTag ?? "!List");
            return;
        }
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
            List<IIdentifiable> reservedIds = new();

            for (var i = 0; i < value.Count; i++)
            {
                var element = value[i];
                if (element is IIdentifiable identifiable
                    && context.Writer.References.Add(identifiable.Id))
                {
                    reservedIds.Add(identifiable);
                }
            }
            List<IIdentifiable> removedIds = new();
            var resultContext = context.BeginSequence(CustomTag ?? "!List", style);
            for (var i = 0; i < value.Count; i++)
            {
                var element = value[i];
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
        var result = context.BeginSequence(CustomTag ?? "!List", style);

        foreach (var x in value)
        {
            result = result.Write(x, DataStyle.Any);
        }
        result.End(context);
    }

    public async ValueTask<List<T?>> Read(Scope scope, List<T?>? parseResult)
    {
        var sequenceScope = scope.As<SequenceScope>();
        var list = new List<T?>();
        var tasks = new List<ValueTask<T?>>();
        foreach (var element in sequenceScope)
        {
            tasks.Add(element.Read<T>());
        }
        foreach( var task in tasks)
        {
            list.Add(await task);
        }
        return list;
    }
}
public class ListSerializerFactory : IYamlSerializerFactory
{
    public void Register(IYamlSerializerResolver resolver)
    {
        resolver.Register(this, typeof(List<>), typeof(List<>));
        resolver.RegisterSerializer(typeof(List<>));
        resolver.RegisterGenericSerializer(typeof(List<>), typeof(ListSerializer<>));

        resolver.Register(this, typeof(List<>), typeof(List<>));

        resolver.RegisterTag("!List", typeof(List<>));
        resolver.Register(this, typeof(List<>), typeof(IList<>));
        resolver.Register(this, typeof(List<>), typeof(ICollection<>));
        resolver.Register(this, typeof(List<>), typeof(IReadOnlyList<>));
        resolver.Register(this, typeof(List<>), typeof(IReadOnlyCollection<>));
        resolver.Register(this, typeof(List<>), typeof(IEnumerable<>));
    }

    public IYamlSerializer Instantiate(Type target)
    {
        var generatorType = typeof(ListSerializer<>);
        var genericParams = target.GenericTypeArguments;
        var genericTypeDef = target.GetGenericTypeDefinition();

        Type filledGeneratorType;
        if (genericTypeDef == typeof(IList<>) ||
            genericTypeDef == typeof(ICollection<>) ||
            genericTypeDef == typeof(IReadOnlyList<>) ||
            genericTypeDef == typeof(IReadOnlyCollection<>) ||
            genericTypeDef == typeof(IEnumerable<>) ||
            genericTypeDef == typeof(List<>))
        {
            filledGeneratorType = generatorType.MakeGenericType(genericParams[0]);
        }
        else
        {
            filledGeneratorType = generatorType.MakeGenericType(genericParams);
        }

        return (IYamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
    }
}
