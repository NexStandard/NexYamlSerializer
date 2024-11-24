namespace NexYaml.Serialization;

public interface IYamlSerializerResolver
{
    YamlSerializer<T> GetSerializer<T>();
    public YamlSerializer? GetSerializer(Type type);
    YamlSerializer GetSerializer(Type type, Type origin);
    YamlSerializer<T>? GetGenericSerializer<T>();
    public Type GetAliasType(string alias);
    public string GetTypeAlias(Type type);
    public void Register(IYamlSerializerFactory yamlFactory, Type target, Type interfaceType);
    public void RegisterSerializer<T>(YamlSerializer<T> serializer);
    public void RegisterSerializer(Type serializer);
    public void RegisterTag(string tag, Type serializerType);
    public void RegisterGenericSerializer(Type target, Type serializerType);
    public static IYamlSerializerResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
}