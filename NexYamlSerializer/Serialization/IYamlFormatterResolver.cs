#nullable enable
using NexYamlSerializer.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace NexVYaml.Serialization
{
    public interface IYamlFormatterResolver
    {
        IYamlFormatter<T> GetFormatter<T>();
        public IYamlFormatter? GetFormatter(Type type);
        IYamlFormatter GetFormatter(Type type,Type origin);
        public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType);
        IYamlFormatter<T>? GetGenericFormatter<T>();
        public Type GetAliasType(string alias);
        public void RegisterFormatter<T>(IYamlFormatter<T> formatter);
        public void RegisterTag(string tag, Type formatterGenericType);
        public void RegisterFormatter(Type formatter);
        public void RegisterGenericFormatter(Type target, Type formatterType);
        public static IYamlFormatterResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
    }

    public static class YamlFormatterResolverExtensions
    {
        public static bool IsNullable(Type value, out Type underlyingType)
        {
            return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
        }
    }
}
