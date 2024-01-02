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
            if (context.SecureMode)
            {
                return NexYamlSerializerRegistry.Instance.GetFormatter<T>().Deserialize(ref parser, context);
            }
            var type = typeof(T);

            parser.TryGetCurrentTag(out var tag);
            IYamlFormatter formatter;
            Type alias;

            alias = NexYamlSerializerRegistry.Instance.GetAliasType(tag.Handle);
            formatter = NexYamlSerializerRegistry.Instance.GetFormatter(alias);

            if (formatter == null)
                return default;
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod(nameof(Deserialize));
            return (T)method.Invoke(formatter, new object[] { parser, context });
        }
        public void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
        {
            if (context.SecureMode)
            {
                var protectedFormatter = NexYamlSerializerRegistry.Instance.GetFormatter<T>();
                protectedFormatter.Serialize(ref emitter, value, context);
                return;
            }
            var type = typeof(T);
            IYamlFormatter formatter;
            if (type.IsInterface || type.IsAbstract)
            {
                formatter = NexYamlSerializerRegistry.Instance.FindFormatter<T>(value.GetType());
                context.IsRedirected = true;
            }
            else if (type.IsGenericType)
            {
                formatter = NexYamlSerializerRegistry.Instance.GetGenericFormatter<T>();
                formatter ??= EmptyFormatter<T>.Empty();
            }
            else
            {
                formatter = NexYamlSerializerRegistry.Instance.GetFormatter<T>();
            }
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod("Serialize");
            method.Invoke(formatter, new object[] { emitter, value, context });
        }
    }
}
