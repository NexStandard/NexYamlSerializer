using NexVYaml.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYamlSerializer.NewYaml;
internal class YamlSerializationReader(YamlParser parser) : ISerializationReader
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
        throw new NotImplementedException();
    }

    public void Serialize(ref sbyte value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref int value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetInt32(out var val))
        {
            value = val;
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect a scalar value as Int32: {parser.CurrentEventType} {parser.currentScalar}");
    }

    public void Serialize(ref uint value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetUInt32(out value))
        {
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect a scalar value as UInt32 : {parser.CurrentEventType} {parser.currentScalar}");
    }

    public void Serialize(ref long value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetInt64(out var val))
        {
            value = val;
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect a scalar value as Int64: {parser.CurrentEventType} {parser.currentScalar}");
    }

    public void Serialize(ref ulong value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetUInt64(out value))
        {
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect a scalar value as UInt64 : {parser.CurrentEventType} ({parser.currentScalar})");
    }

    public void Serialize(ref float value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetFloat(out value))
        {
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect scalar value as float : {parser.CurrentEventType} {parser.currentScalar}");
    }

    public void Serialize(ref double value)
    {
        if (parser.currentScalar is { } scalar && scalar.TryGetDouble(out value))
        {
            return;
        }
        throw new YamlParserException(parser.CurrentMark, $"Cannot detect a scalar value as double : {parser.CurrentEventType} {parser.currentScalar}");
    }

    public void Serialize(ref short value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref ushort value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref char value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref bool value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref string value)
    {
        value = parser.currentScalar?.ToString() ?? string.Empty;
    }

    public void Serialize(ref decimal value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref ReadOnlySpan<byte> value)
    {
        throw new NotImplementedException();
    }
}
