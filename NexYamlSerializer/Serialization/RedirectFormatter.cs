using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NexYamlSerializer;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Serialization.Formatters;

namespace NexVYaml.Serialization
{

    public struct RedirectFormatter<T> : IYamlFormatter<T>
    {
        public T Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            var type = typeof(T);
            parser.TryGetCurrentTag(out var tag);
            IYamlFormatter formatter;
            Type alias;
            
            alias = NexYamlSerializerRegistry.Instance.GetAliasType(tag.Handle);
            formatter = NexYamlSerializerRegistry.Instance.GetFormatter(alias);

            if (formatter == null) return default;


            MethodInfo method = formatter.GetType().GetMethod(nameof(Deserialize));
            return (T)method.Invoke(formatter, new object[] { parser, context });
        }

        public void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
        {
            Type type = typeof(T);
            IYamlFormatter formatter;
            if (type.IsInterface)
            {
                formatter = NexYamlSerializerRegistry.Instance.FindFormatter<T>(value.GetType());
                context.IsRedirected = true;
            }
            else if (type.IsAbstract)
            {
                formatter = NexYamlSerializerRegistry.Instance.FindFormatter<T>(value.GetType());
                context.IsRedirected = true;
            }
            else
            {
                if (type.IsGenericType)
                {
                    formatter = NexYamlSerializerRegistry.Instance.GetGenericFormatter<T>();
                    if (formatter == null)
                        formatter = EmptyFormatter<T>.Empty();
                }
                else
                {
                    formatter = NexYamlSerializerRegistry.Instance.GetFormatter<T>();
                }
            }

            MethodInfo method = formatter.GetType().GetMethod("Serialize");
            method.Invoke(formatter, new object[] { emitter, value, context });
        }
    }
}
