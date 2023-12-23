
using NexYamlSerializer;
using NexYamlSerializer.Serialization.Formatters;
using System;
using NexVYaml.Core;
using NexVYaml.Serialization;

namespace NexVYaml
{
    public class NexYamlSerializerRegistry : IYamlFormatterResolver
    {
        FormatterRegistry FormatterRegistry { get; } = new();
        public static NexYamlSerializerRegistry Instance { get; } = new NexYamlSerializerRegistry();
        
        internal NexYamlSerializerRegistry()
        {

        }
        public Type GetAliasType(string alias) => FormatterRegistry.TypeMap[alias];
        public IYamlFormatter<T>? GetFormatter<T>()
        {
            if (FormatterRegistry.DefinedFormatters.TryGetValue(typeof(T), out var formatter))
            {
                return (IYamlFormatter<T>)formatter;
            }
            return EmptyFormatter<T>.Empty();
        }
        internal IYamlFormatter<T> GetGenericFormatter<T>()
        {
            return (IYamlFormatter<T>)GetGenericFormatter(typeof(T));
        }
        public IYamlFormatter GetGenericFormatter(Type type)
        {
            Type genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type); ;
            if(genericFormatter is null)
                genericFormatter = typeof(EmptyFormatter<>);
            Type genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
            return (IYamlFormatter)Activator.CreateInstance(genericType);
        }

        /// <summary>
        /// Registers all available Serializers, call only once when all assemblies are loaded.
        /// </summary>
        public static void Init()
        {

        }


        internal Type SynchronizeTypes(Type originalType,Type genericTarget)
        {
            return originalType;
        }
        public IYamlFormatter? GetFormatter(Type type)
        {
            if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out IYamlFormatter value))
            {
                return value;
            }
            return GetGenericFormatter(type);
        }
        public void RegisterFormatter<T>(IYamlFormatter<T> formatter)
        {
            Type keyType = typeof(T);
            FormatterRegistry.DefinedFormatters[keyType] = formatter;
        }
        public void RegisterTag(string tag,Type formatterGenericType)
        {
            FormatterRegistry.TypeMap[tag] = formatterGenericType;
        }
        public void RegisterFormatter(Type formatter)
        {
            FormatterRegistry.TypeMap[formatter.FullName] = formatter;
        }
        public void RegisterGenericFormatter(Type target, Type formatterType)
        {
            FormatterRegistry.GenericFormatterBuffer.Add(target, formatterType);
        }
        public void Register<T>(IYamlFormatter<T> formatter, Type interfaceType)
        {
            Type keyType = typeof(T);
            if (!FormatterRegistry.FormatterBuffer.ContainsKey(interfaceType))
            {
                FormatterRegistry.FormatterBuffer.Add(interfaceType, new());
            }
            if (!FormatterRegistry.FormatterBuffer[interfaceType].ContainsKey(keyType))
            {
                FormatterRegistry.FormatterBuffer[interfaceType].Add(keyType, formatter);
            }
            else
            {
                FormatterRegistry.FormatterBuffer[interfaceType][keyType] = formatter;
            }
        }
        public void Register(Type formatterType, Type interfaceType)
        {
            if (!FormatterRegistry.FormatterBuffer.ContainsKey(interfaceType))
            {
                FormatterRegistry.FormatterBuffer.Add(interfaceType, new());
            }
            if (!FormatterRegistry.FormatterBuffer[interfaceType].ContainsKey(formatterType))
            {
                FormatterRegistry.FormatterBuffer[interfaceType].Add(formatterType, null);
            }
            else
            {
                FormatterRegistry.FormatterBuffer[interfaceType][formatterType] = null;
            }
        }
        public IYamlFormatter FindFormatter<T>(Type target)
        {
            if (FormatterRegistry.FormatterBuffer.ContainsKey(typeof(T)))
            {
                if (FormatterRegistry.FormatterBuffer[typeof(T)].TryGetValue(target, out IYamlFormatter value))
                {
                    return FormatterRegistry.FormatterBuffer[typeof(T)][target];
                }
            }
            return GetGenericFormatter(target);
        }
    }
}
