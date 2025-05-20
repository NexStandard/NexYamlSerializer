using NexYaml.Parser;
using NexYaml.Serialization;
using Silk.NET.SDL;
using Stride.Core;

namespace NexYaml.Serializers;

public class ListSerializer<T> : YamlSerializer<List<T>?>
{
    public string? CustomTag { get; init; }
    public override void Write<X>(WriteContext<X> context, List<T>? value, DataStyle style)
    {
        bool hasIdentifiable = false;
        if (value.Count == 0)
        {
            context.WriteEmptySequence(CustomTag ?? "!List");
            return;
        }
        foreach (var item in value)
        {
            if(item is IIdentifiable)
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
                    && !context.Writer.References.Contains(identifiable.Id))
                {
                    context.Writer.References.Add(identifiable.Id);
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
                resultContext = resultContext.Write(element, style);
            }
            resultContext.End(context);
            return;
        }
        var result = context.BeginSequence(CustomTag ?? "!List", style);

        foreach (var x in value)
        {
            result = result.Write(x, style);
        }
        result.End(context);
    }

    public override async ValueTask<List<T>?> Read(IYamlReader stream, ParseContext parseResult)
    {
        var list = new List<T>();
        var tasks = new List<Task<T>>();
        stream.Move(ParseEventType.SequenceStart);
        while (stream.HasSequence)
        {
            tasks.Add(stream.Read<T>(parseResult).AsTask());
        }
        stream.Move(ParseEventType.SequenceEnd);
        return (await Task.WhenAll(tasks)).ToList();
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

        resolver.RegisterTag("List", typeof(List<>));
        resolver.Register(this, typeof(List<>), typeof(IList<>));
        resolver.Register(this, typeof(List<>), typeof(ICollection<>));
        resolver.Register(this, typeof(List<>), typeof(IReadOnlyList<>));
        resolver.Register(this, typeof(List<>), typeof(IReadOnlyCollection<>));
        resolver.Register(this, typeof(List<>), typeof(IEnumerable<>));
    }

    public YamlSerializer Instantiate(Type target)
    {
        if (target.GetGenericTypeDefinition() == typeof(IList<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(List<>))
        {
            var generatorType = typeof(ListSerializer<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        var gen = typeof(ListSerializer<>);
        var genParams = target.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
