using Stride.Core;
using System;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
public interface IYamlReader
{
    public abstract bool IsNull();
    public void Serialize<T>(T value, DataMemberMode mode);
}
