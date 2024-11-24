using NexYaml.Serialization;

namespace NexYaml;
public interface IYamlSerializerFactory
{
    void Register(IYamlSerializerResolver resolver);
    public YamlSerializer Instantiate(Type target);
}
