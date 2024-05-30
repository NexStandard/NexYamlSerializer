using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Linq;

namespace NexVYaml;
public abstract class SerializationWriter : ISerializationStream
{
    public required YamlSerializationContext SerializeContext { get; init; }
    public abstract void WriteTag(string tag);
    public abstract void BeginMapping(DataStyle style);
    public abstract void EndMapping();
    public abstract void BeginSequence(DataStyle style);
    public abstract void EndSequence();
    public abstract void Serialize(ref byte value);
    public abstract void Serialize(ref sbyte value);
    public abstract void Serialize(ref int value);
    public abstract void Serialize(ref uint value);
    public abstract void Serialize(ref long value);
    public abstract void Serialize(ref ulong value);
    public abstract void Serialize(ref float value);
    public abstract void Serialize(ref double value);
    public abstract void Serialize(ref short value);
    public abstract void Serialize(ref ushort value);
    public abstract void Serialize(ref char value);
    public abstract void Serialize(ref bool value);
    public abstract void Serialize(ref string value);
    public abstract void Serialize(ref decimal value);
    public abstract void Serialize(ref ReadOnlySpan<byte> value);
}