using Stride.Core;
using System;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
public interface IYamlReader
{
    public abstract bool IsNull();
    public void Read<T>(T value, DataMemberMode mode)
    {

    }
}
