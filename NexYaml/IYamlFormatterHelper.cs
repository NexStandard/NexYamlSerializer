using NexYaml.Serialization;

namespace NexYaml;
public interface IYamlFormatterHelper
{
    void Register(IYamlFormatterResolver resolver);
    public YamlSerializer Instantiate(Type target);
}
