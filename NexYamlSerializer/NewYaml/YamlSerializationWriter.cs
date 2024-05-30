using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Buffers.Text;
using System.Globalization;

namespace NexVYaml;

public class YamlSerializationWriter : SerializationWriter
{
    Utf8YamlEmitter Emitter { get; set; }

    internal YamlSerializationWriter(Utf8YamlEmitter emitter)
    {
        Emitter = emitter;
    }

    public override void BeginMapping(DataStyle style)
    {
        Emitter.BeginMapping(style);
    }

    public override void BeginSequence(DataStyle style)
    {
        Emitter.BeginSequence(style);
    }

    public override void EndMapping()
    {
        Emitter.EndMapping();
    }

    public override void EndSequence()
    {
        Emitter.EndSequence();
    }

    public override void Serialize(ref int value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref uint value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(10)); // 4294967295

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref long value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(20)); // -9223372036854775808

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref ulong value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(20)); // 18446744073709551615

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref double value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(17));

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref float value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(12));

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref short value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void Serialize(ref ushort value)
    {
        var u = (uint)value;
        Serialize(ref u);
    }

    public override void Serialize(ref byte value)
    {
        var b = (int)value;
        Serialize(ref b);
    }

    public override void Serialize(ref char value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        Emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }
    /// <summary>
    /// Serializes the specified value as the suggested <see cref="ScalarStyle"/> of <see cref="EmitStringAnalyzer.Analyze(string)"/> string.
    /// </summary>
    /// <param name="value">The value to be Serialized to the Stream.</param>
    public override void Serialize(ref string value)
    {
        var result = EmitStringAnalyzer.Analyze(value);
        var style = result.SuggestScalarStyle();
        if(style is ScalarStyle.Plain or ScalarStyle.Any)
        {
            var stringMaxByteCount = StringEncoding.Utf8.GetMaxByteCount(value.Length);
            var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(stringMaxByteCount));
            var offset = 0;
            Emitter.BeginScalar(output, ref offset);
            offset += StringEncoding.Utf8.GetBytes(value, output[offset..]);
            Emitter.EndScalar(output, ref offset);
        }
        else if(ScalarStyle.Folded == style)
        {
            throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
        }
        else if(ScalarStyle.SingleQuoted == style)
        {
            var writer = new StringWriter(Emitter);
            writer.WriteQuotedScalar(value);
        }
        else if(ScalarStyle.DoubleQuoted == style)
        {
            var writer = new StringWriter(Emitter);
            writer.WriteQuotedScalar(value, true);
        }
        else if(ScalarStyle.Literal == style)
        {
            var writer = new StringWriter();
            writer.WriteLiteralScalar(value);
        }
    }
    public override void Serialize(ref bool value)
    {
        Emitter.WriteScalar(value ? YamlCodes.True0 : YamlCodes.False0);
    }

    public override void Serialize(ref ReadOnlySpan<byte> value)
    {
        Emitter.WriteScalar(value);
    }

    public override void Serialize(ref decimal value)
    {
        Span<byte> buf = stackalloc byte[64];

        if (value.TryFormat(buf, out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            Emitter.WriteScalar(buf[..bytesWritten]);
        }
        else
        {
            throw new YamlException($"Cannot serialize a value: {value}");
        }
    }

    public override void Serialize(ref sbyte value)
    {
        var offset = 0;
        var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        Emitter.BeginScalar(output, ref offset);
        if (!value.TryFormat(output[offset..], out var bytesWritten, default, CultureInfo.InvariantCulture))
        {
            throw new YamlException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        Emitter.EndScalar(output, ref offset);
    }

    public override void WriteTag(string tag)
    {
        if (SerializeContext.IsRedirected || SerializeContext.IsFirst)
        {
            var fulTag = $"!{tag}";
            Emitter.Tag(ref fulTag);
            SerializeContext.IsRedirected = false;
            SerializeContext.IsFirst = false;
        }
    }
}
