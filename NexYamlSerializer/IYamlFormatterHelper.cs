using NexVYaml.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml;
public interface IYamlFormatterHelper
{
    void Register(IYamlFormatterResolver resolver);
    public IYamlFormatter Create(Type target);
    public YamlSerializer Instantiate(Type target);
}
