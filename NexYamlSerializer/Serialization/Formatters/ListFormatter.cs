#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Parser;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;

namespace NexVYaml.Serialization;


public class ListFormatter<T> : YamlSerializer<List<T>?>,IYamlFormatter<List<T>?>
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
            var value = context.DeserializeWithAlias<T>(ref parser);
            list.Add(value!);
        }

        parser.ReadWithVerify(ParseEventType.SequenceEnd);
        return list;
    }

    public override void Serialize(ref ISerializationWriter stream, List<T>? value, DataStyle style = DataStyle.Normal)
    {
        stream.Emitter.BeginSequence(style);
        foreach (var x in value)
        {
            stream.Write(x, style);
        }

        stream.Emitter.EndSequence();
    }
}
file class ListHelper : IYamlFormatterHelper
{
    public void Register(IYamlFormatterResolver resolver)
    {
        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(List<>));
        resolver.Register(this, typeof(List<>), typeof(List<>));
        NewSerializerRegistry.Instance.RegisterFormatter(typeof(List<>));
        resolver.RegisterFormatter(typeof(List<>));
        NewSerializerRegistry.Instance.RegisterGenericFormatter(typeof(List<>), typeof(ListFormatter<>));
        resolver.RegisterGenericFormatter(typeof(List<>), typeof(ListFormatter<>));

        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(List<>));
        resolver.Register(this, typeof(List<>), typeof(List<>));

        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(System.Collections.Generic.IList<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IList<>));
        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(System.Collections.Generic.ICollection<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.ICollection<>));
        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyList<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyList<>));
        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyCollection<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IReadOnlyCollection<>));
        NewSerializerRegistry.Instance.Register(this, typeof(List<>), typeof(System.Collections.Generic.IEnumerable<>));
        resolver.Register(this, typeof(List<>), typeof(System.Collections.Generic.IEnumerable<>));

    }
    public IYamlFormatter Create(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ICollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyCollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
        }


        var gen = typeof(ListFormatter<>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (IYamlFormatter)Activator.CreateInstance(fillGen);
    }

    public YamlSerializer Instantiate(Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.ICollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyList<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyCollection<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IEnumerable<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }

        if (type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var generatorType = typeof(ListFormatter<>);
            var genericParams = type.GenericTypeArguments;
            var param = new Type[] { genericParams[0] };
            var filledGeneratorType = generatorType.MakeGenericType(param);
            return (YamlSerializer)Activator.CreateInstance(filledGeneratorType);
        }


        var gen = typeof(ListFormatter<>);
        var genParams = type.GenericTypeArguments;
        var fillGen = gen.MakeGenericType(genParams);
        return (YamlSerializer)Activator.CreateInstance(fillGen);
    }
}
