using System;
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexYamlSerializer.Serialization.Formatters;

namespace NexVYaml.Serialization
{
    public readonly struct RedirectFormatter<T> : IYamlFormatter<T>
    {
        public RedirectFormatter()
        {
        }

        public T Deserialize(ref YamlParser parser, YamlDeserializationContext context)
        {
            if (context.SecureMode)
            {
                return context.Resolver.GetFormatter<T>().Deserialize(ref parser, context);
            }
            var type = typeof(T);

            parser.TryGetCurrentTag(out var tag);
            IYamlFormatter formatter;
            if(tag == null)
            {
                formatter = context.Resolver.GetGenericFormatter<T>();
            }
            else
            {
                Type alias;

                alias = context.Resolver.GetAliasType(tag.Handle);
                formatter = context.Resolver.GetFormatter(alias);
                if (formatter == null)
                {
                    formatter = context.Resolver.GetFormatter(alias, type);
                }
            }



            if (formatter == null)
                return new EmptyFormatter<T>().Deserialize(ref parser, context);
            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod(nameof(Deserialize));
            return (T)method.Invoke(formatter, new object[] { parser, context });
        }

        public readonly void Serialize(ref Utf8YamlEmitter emitter, T? value, YamlSerializationContext context)
        {
            var type = typeof(T);
            if (context.SecureMode)
            {
                if (type.IsGenericType)
                {
                    var protectedGeneric = context.Resolver.GetGenericFormatter<T>();
                    protectedGeneric.Serialize(ref emitter, value!, context);
                }
                else
                {
                    var protectedFormatter = context.Resolver.GetFormatter<T>();
                    protectedFormatter.Serialize(ref emitter, value!, context);

                }
                return;
            }
            var valueType = value!.GetType();
            var formatter = context.Resolver.GetFormatter(value!.GetType(), typeof(T));
            if(valueType != type) 
                context.IsRedirected = true;

            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            var method = formatter.GetType().GetMethod("Serialize");
            method.Invoke(formatter, new object[] { emitter, value, context });
        }
    }
}
