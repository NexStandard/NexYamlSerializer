using NexYaml.Parser;
using Silk.NET.SDL;
using Stride.Core;

namespace NexYaml.Serialization.Formatters;

public class ListFormatter<T> : YamlSerializer<List<T>?>
{
    public override void Write(IYamlWriter stream, List<T>? value, DataStyle style)
    {
        stream.IsRedirected = false;
        if(value!.Any(value => value is IIdentifiable))
        {
            List<IIdentifiable> reservedIds = new();

            for (var i = 0; i < value.Count; i++)
            {
                var element = value[i];
                if (element is IIdentifiable identifiable 
                    && !stream.References.Contains(identifiable.Id))
                {
                    stream.References.Add(identifiable.Id);
                    reservedIds.Add(identifiable);
                }
            }
            List<IIdentifiable> removedIds = new();
            stream.BeginSequence(style);
            for (var i = 0; i < value.Count; i++)
            {
                var element = value[i];
                if(element is IIdentifiable identifiable 
                    && reservedIds.Contains(identifiable) &&
                    !removedIds.Contains(identifiable))
                {
                    stream.References.Remove(identifiable.Id);
                    removedIds.Add(identifiable);
                }
                stream.Write(element, style);
            }
            stream.EndSequence();
            return;
        }
        stream.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Write(x, style);
        }
        stream.EndSequence();
    }

    public override void Read(IYamlReader stream, ref List<T>? value, ref ParseResult result)
    {
        var list = new List<T>();
        stream.ReadWithVerify(ParseEventType.SequenceStart);
        while (stream.HasSequence)
        {
            var val = default(T);
            var parseResult = new ParseResult();
            stream.Read(ref val, ref parseResult);
            int counter = list.Count;
            if (parseResult.IsReference)
            {
                stream.AddReference(parseResult.Reference, (obj) => list[counter] = (T)obj);
            }
            list.Add(val!);
        }
        stream.ReadWithVerify(ParseEventType.SequenceEnd);
        value = list;
    }
}
internal class ListFormatterHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(List<>), typeof(List<>));
        resolver.RegisterFormatter(typeof(List<>));
        resolver.RegisterGenericFormatter(typeof(List<>), typeof(ListFormatter<>));

        resolver.Register(this, typeof(List<>), typeof(List<>));

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
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IReadOnlyList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (target.GetGenericTypeDefinition() == typeof(List<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = target.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        var gen = typeof(ListFormatter<>);
        var genParams = target.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
