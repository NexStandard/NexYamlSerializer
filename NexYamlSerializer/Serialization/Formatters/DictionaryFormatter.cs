#nullable enable
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer;

namespace NexVYaml.Serialization
{
    public class DictionaryFormatter<TKey, TValue> : IYamlFormatter<Dictionary<TKey, TValue>?>
        where TKey : notnull
    {
        public void Serialize(ref Utf8YamlEmitter emitter, Dictionary<TKey, TValue>? value, YamlSerializationContext context)
        {
            if (value == null)
            {
                emitter.WriteNull();
            }
            else
            {
                context.IsRedirected = false;

                IYamlFormatter<TKey> keyFormatter = null;
                IYamlFormatter<TValue> valueFormatter = null;
                if (this.IsPrimitiveType(typeof(TKey)))
                {
                    keyFormatter = context.Resolver.GetFormatter<TKey>();
                }
                if (this.IsPrimitiveType(typeof(TValue)))
                    valueFormatter = context.Resolver.GetFormatter<TValue>();

                if(keyFormatter == null)
                {
                    emitter.BeginSequence();
                    if (value.Count > 0)
                    {
                        foreach (var x in value)
                        {
                            var elementFormatter = context.Resolver.GetFormatterWithVerify<KeyValuePair<TKey, TValue>>();
                            elementFormatter.Serialize(ref emitter, x, context);
                        }
                    }
                    emitter.EndSequence(value.Count == 0);
                }
                else if(valueFormatter == null)
                {
                    emitter.BeginMapping();
                    {
                        foreach (var x in value)
                        {
                            keyFormatter.Serialize(ref emitter, x.Key, context);
                            context.Serialize(ref emitter,x.Value);
                        }
                    }
                    emitter.EndMapping();
                }
                else
                {
                    emitter.BeginMapping();
                    {
                        foreach (var x in value)
                        {
                            keyFormatter.Serialize(ref emitter, x.Key, context);
                            valueFormatter.Serialize(ref emitter, x.Value, context);
                        }
                    }
                    emitter.EndMapping();
                }
            }
        }
        
        public Dictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }
            var map = new Dictionary<TKey, TValue>();
            IYamlFormatter<TKey> keyFormatter = null;
            IYamlFormatter<TValue> valueFormatter = null;

            if (this.IsPrimitiveType(typeof(TKey)))
            {
                keyFormatter = context.Resolver.GetFormatter<TKey>();
            }

            if (keyFormatter == null)
            {
                var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
                var keyValuePairs = context.DeserializeWithAlias(listFormatter, ref parser);
                if (keyValuePairs is not null)
                    return keyValuePairs.ToDictionary();
            }
            else
            {
                parser.ReadWithVerify(ParseEventType.MappingStart);


                while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
                {
                    var key = context.DeserializeWithAlias(keyFormatter, ref parser);
                    var value = context.DeserializeWithAlias<TValue>(ref parser);
                    map.Add(key, value);
                }

                parser.ReadWithVerify(ParseEventType.MappingEnd);
                return map;
            }
            return map;
        }
    }
    file class DictionaryFormatterHelper : IYamlFormatterHelper
    {
        public void Register(IYamlFormatterResolver resolver)
        {
            resolver.RegisterTag($"Dictionary,NexYamlTest", typeof(Dictionary<,>));
            resolver.Register(this, typeof(Dictionary<,>), typeof(Dictionary<,>));
            resolver.RegisterGenericFormatter(typeof(Dictionary<,>), typeof(DictionaryFormatter<,>));
            resolver.RegisterFormatter(typeof(Dictionary<,>));


            resolver.Register(this, typeof(Dictionary<,>), typeof(System.Collections.Generic.IDictionary<,>));
            resolver.Register(this, typeof(Dictionary<,>), typeof(System.Collections.Generic.IReadOnlyDictionary<,>));

        }
        public IYamlFormatter Create(Type type)
        {
            if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IDictionary<,>))
            {
                var generatorType = typeof(DictionaryFormatter<,>);
                var genericParams = type.GenericTypeArguments;
                var param = new Type[] { genericParams[0], genericParams[1] };
                var filledGeneratorType = generatorType.MakeGenericType(param);
                return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
            }

            if (type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.IReadOnlyDictionary<,>))
            {
                var generatorType = typeof(DictionaryFormatter<,>);
                var genericParams = type.GenericTypeArguments;
                var param = new Type[] { genericParams[0], genericParams[1] };
                var filledGeneratorType = generatorType.MakeGenericType(param);
                return (IYamlFormatter)Activator.CreateInstance(filledGeneratorType);
            }


            var gen = typeof(DictionaryFormatter<,>);
            var genParams = type.GenericTypeArguments;
            var fillGen = gen.MakeGenericType(genParams);
            return (IYamlFormatter)Activator.CreateInstance(fillGen);
        }
    }
}

