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
            if (tag.Handle.Contains('$'))
            {
                string validGenericTag = tag.Handle.Replace('$', '`');
                string splitted = validGenericTag.Substring(0, validGenericTag.IndexOf('`') + 2);

                var x = ExtractGenericParameters(validGenericTag);
                var startType = FindStartType(splitted);
                var b = NexYamlSerializerRegistry.Instance.FindGenericFormatter(startType);
                alias = b.MakeGenericType(x.ToArray());
                formatter = (IYamlFormatter)Activator.CreateInstance(alias);
                NexYamlSerializerRegistry.Instance.RequestGenericBufferStorage(formatter, startType);
            }
            else
            {
                alias = NexYamlSerializerRegistry.Instance.GetAliasType(tag.Handle);
                formatter = NexYamlSerializerRegistry.Instance.GetFormatter(alias);
            }
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
        public static List<Type> ExtractGenericParameters(string typeName)
        {
            // Use a regular expression to extract the generic part
            // The pattern matches everything within angle brackets <>
            string pattern = @"\[([^]]*)\]";
            Match match = Regex.Match(typeName, pattern);

            List<Type> genericParameters = new List<Type>();

            if (match.Success)
            {
                string genericPart = match.Groups[1].Value;

                // Split the generic part by comma to get individual parameter types
                string[] parameterTypeNames = genericPart.Split(',');

                foreach (string parameterTypeName in parameterTypeNames)
                {
                    // Trim whitespace and convert the parameter type name to Type
                    string trimmedTypeName = parameterTypeName.Trim();
                    Type parameterType = null;

                    // Iterate through all loaded assemblies in the current application domain
                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        parameterType = assembly.GetType(trimmedTypeName);

                        if (parameterType != null)
                        {
                            genericParameters.Add(parameterType);
                            break;  // Found the type, no need to search further
                        }
                    }

                    // Handle the case where the type couldn't be resolved
                    if (parameterType == null)
                    {
                        // You can handle this case as needed, e.g., throw an exception
                        throw new TypeLoadException($"Unable to resolve type: {trimmedTypeName}");
                    }
                }
            }

            return genericParameters;
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
