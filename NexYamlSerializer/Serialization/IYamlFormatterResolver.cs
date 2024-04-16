#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace NexVYaml.Serialization;

public interface IYamlFormatterResolver
{
    YamlSerializer<T> GetFormatter<T>();
    public YamlSerializer? GetFormatter(Type type);
    YamlSerializer GetFormatter(Type type, Type origin);
    public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType);
    YamlSerializer<T>? GetGenericFormatter<T>();
    public Type GetAliasType(string alias);
    public void RegisterFormatter<T>(YamlSerializer<T> formatter);
    public void RegisterTag(string tag, Type formatterGenericType);
    public void RegisterFormatter(Type formatter);
    public void RegisterGenericFormatter(Type target, Type formatterType);
    public static IYamlFormatterResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
}

public static class YamlFormatterResolverExtensions
{
    public static bool IsNullable(Type value, [NotNullWhen(true)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)!) != null;
    }
}
