using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
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
    protected Utf8YamlEmitter Emitter { get; }
    void Serialize(ReadOnlySpan<byte> value);
    void SerializeTag(ref string tag);
    void BeginMapping(DataStyle style);
    void EndMapping();
    void BeginSequence(DataStyle style);
    void EndSequence();
    public void Serialize(string key, sbyte value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, int value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, uint value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, long value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, ulong value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, float value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, double value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, short value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, ushort value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, char value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, bool value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, string value)
    {
        Serialize(ref key);
        if(value is null)
        {
            Serialize(YamlCodes.Null0);
        }
        else
        {
            Serialize(ref value);
        }
    }

    public void Serialize(string key, decimal value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, byte value)
    {
        Serialize(ref key);
        Serialize(ref value);
    }
}