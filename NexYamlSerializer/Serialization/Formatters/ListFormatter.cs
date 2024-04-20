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
            context.DeserializeWithAlias<T>(ref parser, ref value);
            list.Add(value!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ISerializationWriter stream, List<T> value, DataStyle style = DataStyle.Normal)
    {
        ListFormatterHelper.Serialize(stream, value, style);
    }
}
internal class ListFormatterHelper : IYamlFormatterHelper
{
    public static void Serialize<T>(ISerializationWriter stream, List<T> value, DataStyle style = DataStyle.Normal)
    {
        stream.BeginSequence(style);
        if (typeof(T).IsValueType || typeof(T) == typeof(string))
        {
            var y = stream.SerializeContext.Resolver.GetFormatter<T>();
            foreach (var x in value)
            {
                y.Serialize(stream, x, style);
            }
        }
        else
        {
            foreach (var x in value)
            {
                stream.Write(x, style);
            }
        }

        stream.EndSequence();
    }
    public void Register(IYamlFormatterResolver resolver)
    {
        resolver.Register(this, typeof(List<>), typeof(List<>));
        resolver.RegisterFormatter(typeof(List<>));
        resolver.RegisterGenericFormatter(typeof(List<>), typeof(ListFormatter<>));

        resolver.Register(this, typeof(List<>), typeof(List<>));

        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IList<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.ICollection<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyList<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyCollection<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IEnumerable<>));
    }

    public YamlSerializer Instantiate(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ICollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyCollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        if (type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType)!;
        }

        var gen = typeof(ListFormatter<>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen)!;
    }
}
