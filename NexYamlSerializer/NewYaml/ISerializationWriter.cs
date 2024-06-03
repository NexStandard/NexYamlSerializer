using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Linq;

namespace NexVYaml;
public interface ISerializationWriter : ISerializationStream
{
    public YamlSerializationContext SerializeContext { get; init; }
    public abstract void WriteTag(string tag);
    public abstract void BeginMapping(DataStyle style);
    public abstract void EndMapping();
    public abstract void BeginSequence(DataStyle style);
    public abstract void EndSequence();
}