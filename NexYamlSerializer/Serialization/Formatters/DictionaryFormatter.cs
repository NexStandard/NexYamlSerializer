#nullable enable
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
{
    public class DictionaryFormatter<TKey, TValue> : IYamlFormatter<Dictionary<TKey, TValue>?>
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
                    List<KeyValuePair<TKey,TValue>> valuepairs = new List<KeyValuePair<TKey,TValue>>(value.AsEnumerable());
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
}

