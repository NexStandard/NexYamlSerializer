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
    void Serialize(ReadOnlySpan<byte> value);
    void WriteTag(string tag);
    void BeginMapping(DataStyle style);
    void EndMapping();
    void BeginSequence(DataStyle style);
    void EndSequence();
}