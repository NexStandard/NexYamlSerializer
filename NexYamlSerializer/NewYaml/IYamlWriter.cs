using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Linq;

namespace NexVYaml;
public interface IYamlWriter : IYamlStream
{
    public void WriteTag(string tag);
    public void BeginMapping(DataStyle style);
    public void EndMapping();
    public void BeginSequence(DataStyle style);
    public void EndSequence();
    public void Serialize<T>(ref T value, DataStyle style);
}