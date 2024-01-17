using NexVYaml.Emitter;
using NexVYaml.Internal;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml.Emitter;
public static class UTF8YamlEmitterExtensions
{
    public static void WriteBool(this Utf8YamlEmitter emitter, bool value)
    {
        emitter.WriteScalar(value ? YamlCodes.True0 : YamlCodes.False0);
    }
    public static void WriteNull(this Utf8YamlEmitter emitter)
    {
        emitter.WriteScalar(YamlCodes.Null0);
    }
    public static void WriteInt32(this Utf8YamlEmitter emitter,int value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(11)); // -2147483648

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteUInt32(this Utf8YamlEmitter emitter,uint value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(10)); // 4294967295

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteInt64(this Utf8YamlEmitter emitter,long value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(20)); // -9223372036854775808

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteUInt64(this Utf8YamlEmitter emitter,ulong value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(20)); // 18446744073709551615

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteFloat(this Utf8YamlEmitter emitter,float value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(12));

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteDouble(this Utf8YamlEmitter emitter, double value)
    {
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(17));

        emitter.BeginScalar(output, ref offset);
        if (!Utf8Formatter.TryFormat(value, output[offset..], out var bytesWritten))
        {
            throw new YamlEmitterException($"Failed to emit : {value}");
        }
        offset += bytesWritten;
        emitter.EndScalar(output, ref offset);
    }
    public static void WriteString(this Utf8YamlEmitter emitter,string value, ScalarStyle style = ScalarStyle.Any)
    {
        if (style == ScalarStyle.Any)
        {
            var analyzeInfo = EmitStringAnalyzer.Analyze(value);
            style = analyzeInfo.SuggestScalarStyle();
        }
        var writer = new StringWriter(emitter);
        switch (style)
        {
            case ScalarStyle.Plain:
                writer.WritePlainScalar(ref value);
                break;
            case ScalarStyle.SingleQuoted:
                writer.WriteQuotedScalar(value, doubleQuote: false);
                break;
            case ScalarStyle.DoubleQuoted:
                writer.WriteQuotedScalar(value, doubleQuote: true);
                break;
            case ScalarStyle.Literal:
                writer.WriteLiteralScalar(value);
                break;
            case ScalarStyle.Folded:
                throw new NotSupportedException();

            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }
}
public static class ExperimentalExtensions
{
    /// <summary>
    /// Experimental Feature to write Custom Yaml
    /// writes it in the style of "key : value"
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void WriteMapping(this Utf8YamlEmitter emitter, string key, int value,bool lineBreak=true)
    {
        emitter.WriteString(key, ScalarStyle.Plain);
        emitter.WriteString(" : ", ScalarStyle.Plain);
        emitter.WriteInt32(value);
        if (lineBreak)
            emitter.WriteString("\n");
    }
    /// <summary>
    /// Experimental Feature to write Custom Yaml
    /// writes it in the style of "key : value"
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void WriteMapping(this Utf8YamlEmitter emitter, string key, float value,bool lineBreak=true)
    {
        emitter.WriteString(key, ScalarStyle.Plain);
        emitter.WriteString(" : ", ScalarStyle.Plain);
        emitter.WriteFloat(value);
        if (lineBreak)
            emitter.WriteString("\n");
    }

    /// <summary>
    /// Experimental Feature to write Custom Yaml
    /// writes it in the style of "key: value"
    /// Based on the <paramref name="style"/> this output can change
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void WriteMapping(this Utf8YamlEmitter emitter, string key, string value, ScalarStyle style = ScalarStyle.Plain, bool lineBreak = true)
    {
        emitter.WriteString(key, ScalarStyle.Plain);
        emitter.WriteString(": ", ScalarStyle.Plain);
        emitter.WriteString(value,style);
        if (lineBreak)
            emitter.WriteString("\n");
    }

    /// <summary>
    /// Experimental Feature to write Custom Yaml
    /// writes it in the style of "key: value"
    /// Based on the <paramref name="style"/> this output can change
    /// </summary>
    /// <param name="emitter"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void WriteSequenceElement(this Utf8YamlEmitter emitter, string key, string value, ScalarStyle style = ScalarStyle.Plain, bool lineBreak = true)
    {
        emitter.WriteString("- ");
        emitter.WriteString(key, ScalarStyle.Plain);
        emitter.WriteString(": ", ScalarStyle.Plain);
        emitter.WriteString(value,style);
        if(lineBreak)
            emitter.WriteString("\n");
    }
}
unsafe ref struct StringWriter(Utf8YamlEmitter emitter)
{
    public readonly void WritePlainScalar(ref string value)
    {
        var stringMaxByteCount = StringEncoding.Utf8.GetMaxByteCount(value.Length);
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(stringMaxByteCount));
        var offset = 0;
        emitter.BeginScalar(output, ref offset);
        offset += StringEncoding.Utf8.GetBytes(value, output[offset..]);
        emitter.EndScalar(output, ref offset);
    }
    public readonly unsafe void WriteQuotedScalar(string value, bool doubleQuote = true)
    {
        var scalarStringBuilt = EmitStringAnalyzer.BuildQuotedScalar(value, doubleQuote);
        Span<char> scalarChars = stackalloc char[scalarStringBuilt.Length];
        scalarStringBuilt.CopyTo(0, scalarChars, scalarStringBuilt.Length);

        var maxByteCount = StringEncoding.Utf8.GetMaxByteCount(scalarChars.Length);
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(maxByteCount));
        emitter.BeginScalar(output, ref offset);
        offset += StringEncoding.Utf8.GetBytes(scalarChars, output[offset..]);
        emitter.EndScalar(output, ref offset);
    }
    public readonly unsafe void WriteLiteralScalar(string value)
    {
        var indentCharCount = (emitter.CurrentIndentLevel + 1) * emitter.Options.IndentWidth;
        var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
        Span<char> scalarChars = stackalloc char[scalarStringBuilt.Length];
        scalarStringBuilt.CopyTo(0, scalarChars, scalarStringBuilt.Length);

        if (emitter.StateStack.Current is EmitState.BlockMappingValue or EmitState.BlockSequenceEntry)
        {
            scalarChars = scalarChars[..^1]; // Remove duplicate last line-break;
        }

        var maxByteCount = StringEncoding.Utf8.GetMaxByteCount(scalarChars.Length);
        var offset = 0;
        var output = emitter.Writer.GetSpan(emitter.CalculateMaxScalarBufferLength(maxByteCount));
        emitter.BeginScalar(output, ref offset);
        offset += StringEncoding.Utf8.GetBytes(scalarChars, output[offset..]);
        emitter.EndScalar(output, ref offset);
    }
}
