using NexVYaml;
using NexVYaml.Parser;
using System;
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
        throw new NotImplementedException();
    }

    public override void Serialize(ref sbyte value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref int value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetInt32(out var val))
        {
            value = val;
            return;
        }
        value = default;
    }

    public override void Serialize(ref uint value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetUInt32(out value))
        {
            return;
        }
        value = default;
    }

    public override void Serialize(ref long value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetInt64(out var val))
        {
            value = val;
            return;
        }
        throw new YamlException(parser.CurrentMark, $"Cannot detect a scalar value as Int64: {parser.CurrentEventType} {parser.currentScalar}");
    }

    public override void Serialize(ref ulong value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetUInt64(out value))
        {
            return;
        }
        throw new YamlException(parser.CurrentMark, $"Cannot detect a scalar value as UInt64 : {parser.CurrentEventType} ({parser.currentScalar})");
    }

    public override void Serialize(ref float value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetFloat(out value))
        {
            return;
        }
        throw new YamlException(parser.CurrentMark, $"Cannot detect scalar value as float : {parser.CurrentEventType} {parser.currentScalar}");
    }

    public override void Serialize(ref double value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetDouble(out value))
        {
            return;
        }
        throw new YamlException(parser.CurrentMark, $"Cannot detect a scalar value as double : {parser.CurrentEventType} {parser.currentScalar}");
    }

    public override void Serialize(ref short value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref ushort value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref char value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref bool value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref string value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref decimal value)
    {
        throw new NotImplementedException();
    }

    public override void Serialize(ref ReadOnlySpan<byte> value)
    {
        throw new NotImplementedException();
    }
}
