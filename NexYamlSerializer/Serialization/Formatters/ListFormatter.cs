#nullable enable
using System;
using System.Collections.Generic;
using System.Text;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Parser;

namespace NexVYaml.Serialization
{

    public class ListFormatter<T> : IYamlFormatter<List<T>?>
    {
        public void Serialize(ref Utf8YamlEmitter emitter, List<T>? value, YamlSerializationContext context)
        {
            if (value is null)
            {
                emitter.WriteNull();
            }
            else
            {
                context.IsRedirected = false;
                emitter.BeginSequence();
                if (value.Count > 0)
                {
                    var elementFormatter = context.Resolver.GetFormatterWithVerify<T>();
                    foreach (var x in value)
                    {
                        elementFormatter.Serialize(ref emitter, x, context);
                    }
                }
                emitter.EndSequence(value.Count == 0);
            }
        }

        public List<T>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }

            parser.ReadWithVerify(ParseEventType.SequenceStart);

            var list = new List<T>();
            var elementFormatter = context.Resolver.GetFormatterWithVerify<T>();
            while (!parser.End && parser.CurrentEventType != ParseEventType.SequenceEnd)
            {
                var value = context.DeserializeWithAlias(elementFormatter, ref parser);
                list.Add(value);
            }

            parser.ReadWithVerify(ParseEventType.SequenceEnd);
            return list;
        }

    }
    file class ListHelper : IYamlFormatterHelper
    {
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
    }
}
