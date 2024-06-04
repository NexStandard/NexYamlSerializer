using System;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
public interface IYamlReader :  IYamlStream
{
    public abstract bool IsNull();
}
