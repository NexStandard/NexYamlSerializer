using System;
using System.Linq;

namespace NexYamlSerializer.NewYaml;
internal abstract class SerializationReader :  ISerializationStream
{
    public abstract bool IsNull();
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
    public abstract void Serialize(ref string? value);
    public abstract void Serialize(ref decimal value);
    public abstract void Serialize(ref ReadOnlySpan<byte> value);
}
