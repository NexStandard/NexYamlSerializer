using NexVYaml;
using NexVYaml.Parser;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.NewYaml;
internal class YamlSerializationReader(YamlParser parser) : SerializationReader
{
    public override bool IsNull()
    {
        if (parser.IsNullScalar())
        {
            parser.Read();
            return true;
        }
        return false;
    }

    public override void Serialize(ref byte value)
    {
        if(parser.TryGetScalarAsUInt32(out var result))
        {
            value = checked((byte)result);
        }
    }

    public override void Serialize(ref sbyte value)
    {
        if(parser.TryGetScalarAsInt32(out var result))
        {
            value = checked((sbyte)result);
        }
    }

    public override void Serialize(ref int value)
    {
        if (parser.TryGetScalarAsInt32(out var val))
        {
            value = val;
            return;
        }
    }

    public override void Serialize(ref uint value)
    {
        if (parser.TryGetScalarAsUInt32(out value))
        {
            return;
        }
    }

    public override void Serialize(ref long value)
    {
        if (parser.TryGetScalarAsInt64(out var result))
        {
            value = result;
            return;
        }
    }

    public override void Serialize(ref ulong value)
    {
        if (parser.TryGetScalarAsUInt64(out value))
        {
            return;
        }
    }

    public override void Serialize(ref float value)
    {
        if (parser.TryGetScalarAsFloat(out value))
        {
            return;
        }
    }

    public override void Serialize(ref double value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetDouble(out value))
        {
            return;
        }
    }

    public override void Serialize(ref short value)
    {
        var result = parser.GetScalarAsInt32();
        parser.Read();
        value = checked((short)result);
    }

    public override void Serialize(ref ushort value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((ushort)result);
    }

    public override void Serialize(ref char value)
    {
        var result = parser.GetScalarAsUInt32();
        parser.Read();
        value = checked((char)result);
    }

    public override void Serialize(ref bool value)
    {
        var result = parser.GetScalarAsBool();
        parser.Read();
        value = result;
    }

    public override void Serialize(ref string? value)
    {
        value = parser.ReadScalarAsString();
    }

    public override void Serialize(ref decimal value)
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

    public override void Serialize(ref ReadOnlySpan<byte> value)
    {
        parser.TryGetScalarAsSpan(out value);
        
    }
}
