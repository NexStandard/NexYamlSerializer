#nullable enable
using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Collections.Generic;

namespace NexVYaml.Serialization;

public class ListFormatter<T> : YamlSerializer<List<T>?>
{
    public override List<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return default;
        }

        parser.ReadWithVerify(ParseEventType.SequenceStart);

        var list = new List<T>();
        while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
        {
            var value = default(T);
            context.DeserializeWithAlias(ref parser, ref value);
            list.Add(value!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ISerializationWriter stream, List<T>? value, DataStyle style)
    {
        ListFormatterHelper.Serialize(stream, value!, style);
    }
}
internal class ListFormatterHelper : IYamlFormatterHelper
{
    public static void Serialize<T>(ISerializationWriter stream, List<T> value, DataStyle style)
    {
        stream.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Write(x, style);
        }
        stream.EndSequence();
    }
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
