using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;

public class ListSerializer<T> : IYamlSerializer<List<T?>>
{
    public string? CustomTag { get; init; }

    public void Write<X>(WriteContext<X> context, List<T?> value, DataStyle style) where X : Node
    {
        CollectionSerialization.WriteCollection<X, T, List<T?>>(context, value, style, "!List");
    }

    public async ValueTask<List<T?>> Read(Scope scope, List<T?>? parseResult)
    {
        var list =  parseResult ?? new List<T?>();
        list.Clear();

        await CollectionSerialization.ReadCollection<T, List<T?>>(scope, list);
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
