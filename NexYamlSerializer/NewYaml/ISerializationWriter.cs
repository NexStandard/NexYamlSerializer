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
    void SerializeTag(ref string tag);
    void BeginMapping(DataStyle style);
    void EndMapping();
    void BeginSequence(DataStyle style);
    void EndSequence();
    public void Serialize(string key, sbyte value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, int value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, uint value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, long value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, ulong value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, float value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, double value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, short value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, ushort value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, char value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, bool value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, string value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        if (value is null)
        {
            Serialize(YamlCodes.Null0);
        }
        else
        {
            Serialize(ref value);
        }
    }

    public void Serialize(string key, decimal value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }

    public void Serialize(string key, byte value, DataStyle style = DataStyle.Any)
    {
        Serialize(ref key);
        Serialize(ref value);
    }
}