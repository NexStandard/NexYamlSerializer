using NexYaml.Serialization;
using System;

namespace NexYaml;
public interface IYamlFormatterHelper
{
    void Register(IYamlFormatterResolver resolver);
    public YamlSerializer Instantiate(Type target);
}
