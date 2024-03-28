using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Serialization;
using NexYamlSerializer.Emitter.Serializers;
using System;
using System.Buffers.Text;
using ScalarStyle = NexVYaml.Emitter.ScalarStyle;

namespace NexYaml2.NewYaml;
internal class YamlSerializationWriter : IYamlStream
{
    public Utf8YamlEmitter Emitter { get; set; }
    public YamlSerializationContext SerializeContext { get; init; }

    public void Serialize(ref int value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref uint value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(10)); // 4294967295

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref long value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(20)); // -9223372036854775808

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref ulong value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(20)); // 18446744073709551615

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref double value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(17));

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref float value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(12));

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public void Serialize(ref short value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref ushort value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref byte value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(ref char value)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Serializes the specified value as <see cref="ScalarStyle.Plain"/> string.
    /// </summary>
    /// <param name="value">The value to be Serialized to the Stream.</param>
    public void Serialize(ref string value)
    {
        var stringMaxByteCount = StringEncoding.Utf8.GetMaxByteCount(value.Length);
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(stringMaxByteCount));
        var offset = 0;
        Emitter.BeginScalar(output, ref offset);
        offset += StringEncoding.Utf8.GetBytes(value, output[offset..]);
        Emitter.EndScalar(output, ref offset);
    }
    public void Serialize(ref bool value)
    {
        Emitter.WriteScalar(value ? YamlCodes.True0 : YamlCodes.False0);
    }

    public void Serialize(ReadOnlySpan<byte> value)
    {
        Emitter.WriteScalar(value);
    }
}
