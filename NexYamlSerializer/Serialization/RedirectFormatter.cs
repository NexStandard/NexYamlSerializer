using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using VYaml.Emitter;
using VYaml.Parser;

namespace VYaml.Serialization
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
            


            MethodInfo method = formatter.GetType().GetMethod(nameof(Deserialize));
            return (T)method.Invoke(formatter, new object[] { parser, context });
        }
        private Type FindStartType(string query)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type t = assembly.GetType(query);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context)
        {
            Type type = typeof(T);
            IYamlFormatter formatter;
            if (type.IsInterface)
            {
                formatter = NexYamlSerializerRegistry.Instance.FindInterfaceTypeBased<T>(value.GetType());
                context.IsRedirected = true;
            }
            else if (type.IsAbstract)
            {
                formatter = NexYamlSerializerRegistry.Instance.FindAbstractTypeBased<T>(value.GetType());
                context.IsRedirected = true;
            }
            else
            {
                if (type.IsGenericType)
                {
                    Type formatterType = NexYamlSerializerRegistry.Instance.FindGenericFormatter<T>();
                    Type closedType = formatterType.MakeGenericType(typeof(T).GenericTypeArguments);
                    formatter = (IYamlFormatter)Activator.CreateInstance(closedType);
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
