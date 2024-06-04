using NexVYaml;
using NexVYaml.Parser;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.NewYaml;
internal class YamlReader(YamlParser parser) : IYamlReader
{
    public bool IsNull()
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return true;
        }
        return false;
    }

    public void Serialize(ref byte value)
    {
        if(parser.TryGetScalarAsUInt32(out var result))
        {
            value = checked((byte)result);
        }
    }

    public void Serialize(ref sbyte value)
    {
        if(parser.TryGetScalarAsInt32(out var result))
        {
            value = checked((sbyte)result);
        }
    }

    public void Serialize(ref int value)
    {
        if (parser.TryGetScalarAsInt32(out var val))
        {
            value = val;
            return;
        }
    }

    public void Serialize(ref uint value)
    {
        if (parser.TryGetScalarAsUInt32(out value))
        {
            return;
        }
    }

    public void Serialize(ref long value)
    {
        if (parser.TryGetScalarAsInt64(out var result))
        {
            value = result;
            return;
        }
    }

    public void Serialize(ref ulong value)
    {
        if (parser.TryGetScalarAsUInt64(out value))
        {
            return;
        }
    }

    public void Serialize(ref float value)
    {
        if (parser.TryGetScalarAsFloat(out value))
        {
            return;
        }
    }

    public void Serialize(ref double value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetDouble(out value))
        {
            return;
        }
    }

    public void Serialize(ref short value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = checked((short)result);
    }

    public void Serialize(ref ushort value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((ushort)result);
    }

    public void Serialize(ref char value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((char)result);
    }

    public void Serialize(ref bool value)
    {
        var result = parser.GetScalarAsBool();
        parser.Read();
        value = result;
    }

    public void Serialize(ref string? value)
    {
        value = parser.ReadScalarAsString();
    }

    public void Serialize(ref decimal value)
    {
        if (parser.TryGetScalarAsSpan(out var span) &&
                  Utf8Parser.TryParse(span, out decimal val, out var bytesConsumed) &&
                  bytesConsumed == span.Length)
        {
            parser.Read();
            value = val;
            return;
        }
    }

    public void Serialize(ref ReadOnlySpan<byte> value)
    {
        parser.TryGetScalarAsSpan(out value);
        
    }
}
