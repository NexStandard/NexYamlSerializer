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
                return context.Resolver.GetFormatter<T>().Deserialize(ref parser, context);
            }
            var type = typeof(T);

            parser.TryGetCurrentTag(out var tag);
            IYamlFormatter formatter;
            Type alias;

            alias = context.Resolver.GetAliasType(tag.Handle);
            formatter = context.Resolver.GetFormatter(alias);

            if (formatter == null)
            {
                formatter = context.Resolver.GetGenericFormatter(alias, type);
            }
            if (formatter == null)
                return new EmptyFormatter<T>().Deserialize(ref parser, context);
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod(nameof(Deserialize));
            return (T)method.Invoke(formatter, new object[] { parser, context });
        }
        public readonly void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
        {
            if (context.SecureMode)
            {
                var protectedFormatter = context.Resolver.GetFormatter<T>();
                protectedFormatter.Serialize(ref emitter, value, context);
                return;
            }
            var type = typeof(T);
            IYamlFormatter formatter;
            if (type.IsInterface || type.IsAbstract)
            {
                formatter = context.Resolver.FindFormatter<T>(value.GetType());
                context.IsRedirected = true;
            }
            else if (type.IsGenericType)
            {
                formatter = context.Resolver.GetGenericFormatter<T>();
                formatter ??= EmptyFormatter<T>.Empty();
            }
            else
            {
                formatter = context.Resolver.GetFormatter<T>();
            }
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod("Serialize");
            method.Invoke(formatter, new object[] { emitter, value, context });
        }
    }
}
