using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml;
public interface ISerializationWriter : ISerializationStream
{
    public YamlSerializationContext SerializeContext { get; init; }
    public Utf8YamlEmitter Emitter { get; }
    void Serialize(ReadOnlySpan<byte> value);
    void BeginMapping(DataStyle style);
    void EndMapping();
    void BeginSequence(DataStyle style);
    void EndSequence();
}