using NexVYaml.Serialization;
using System;

namespace NexVYaml;
public interface IYamlFormatterHelper
{
    void Register(IYamlFormatterResolver resolver);
    public YamlSerializer Instantiate(Type target);
}
