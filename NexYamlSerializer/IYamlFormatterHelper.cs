using NexVYaml.Serialization;
using System;
using System.Linq;

namespace NexVYaml;
public interface IYamlFormatterHelper
{
    void Register(IYamlFormatterResolver resolver);
    public YamlSerializer Instantiate(Type target);
}
