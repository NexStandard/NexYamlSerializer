#nullable enable
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using NexVYaml.Emitter;
using NexVYaml.Parser;

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
                if (IsPrimitiveType(typeof(TKey)))
                {
                    var keyFormatter = context.Resolver.GetFormatterWithVerify<TKey>();
                    var valueFormatter = context.Resolver.GetFormatterWithVerify<TValue>();

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
                else
                {
                    var valuepairs = new List<KeyValuePair<TKey,TValue>>(value.AsEnumerable());
                    var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
                    listFormatter.Serialize(ref emitter, valuepairs, context);
                }
            }
        }
        private bool IsPrimitiveType(Type type)
        {
            return type.IsPrimitive ||
                   type == typeof(bool) ||
                   type == typeof(byte) ||
                   type == typeof(sbyte) ||
                   type == typeof(char) ||
                   type == typeof(short) ||
                   type == typeof(ushort) ||
                   type == typeof(int) ||
                   type == typeof(uint) ||
                   type == typeof(long) ||
                   type == typeof(ulong) ||
                   type == typeof(float) ||
                   type == typeof(double) ||
                   type == typeof(decimal) ||
                   type == typeof(string) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan);
        }
        public Dictionary<TKey, TValue>? Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (parser.IsNullScalar())
            {
                parser.Read();
                return default;
            }
            var map = new Dictionary<TKey, TValue>();
            if (IsPrimitiveType(typeof(TKey)))
            {
                parser.ReadWithVerify(ParseEventType.MappingStart);

                var keyFormatter = context.Resolver.GetFormatterWithVerify<TKey>();
                var valueFormatter = context.Resolver.GetFormatterWithVerify<TValue>();

                while (!parser.End && parser.CurrentEventType != ParseEventType.MappingEnd)
                {
                    var key = context.DeserializeWithAlias(keyFormatter, ref parser);
                    var value = context.DeserializeWithAlias(valueFormatter, ref parser);
                    map.Add(key, value);
                }

                parser.ReadWithVerify(ParseEventType.MappingEnd);
            }
            else
            {
                var listFormatter = new ListFormatter<KeyValuePair<TKey, TValue>>();
                var keyValuePairs = context.DeserializeWithAlias(listFormatter,ref parser);
                if(keyValuePairs is not null)
                    map = keyValuePairs.ToDictionary();
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

