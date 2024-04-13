using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.NewYaml;
public interface ISerializationStream
{
    void Serialize(ref byte value);
    void Serialize(ref sbyte value);
    void Serialize(ref int value);
    void Serialize(ref uint value);
    void Serialize(ref long value);
    void Serialize(ref ulong value);
    void Serialize(ref float value);
    void Serialize(ref double value);
    void Serialize(ref short value);
    void Serialize(ref ushort value);

    void Serialize(ref char value);
    void Serialize(ref bool value);
    void Serialize(ref string value);
    void Serialize(ref decimal value);
    void Serialize<T>(T value, DataStyle style = DataStyle.Any);
}
