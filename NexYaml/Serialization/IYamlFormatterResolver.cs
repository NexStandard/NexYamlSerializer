namespace NexYaml.Serialization;

public interface IYamlFormatterResolver
{
    YamlSerializer<T> GetFormatter<T>();
    public YamlSerializer? GetFormatter(Type type);
    YamlSerializer GetFormatter(Type type, Type origin);
    YamlSerializer<T>? GetGenericFormatter<T>();
    public Type GetAliasType(string alias);
    public string GetTypeAlias(Type type);
    public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType);
    public void RegisterFormatter<T>(YamlSerializer<T> formatter);
    public void RegisterFormatter(Type formatter);
    public void RegisterTag(string tag, Type formatterGenericType);
    public void RegisterGenericFormatter(Type target, Type formatterType);
    public static IYamlFormatterResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
}