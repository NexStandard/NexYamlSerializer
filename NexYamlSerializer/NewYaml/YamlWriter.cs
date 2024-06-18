using NexVYaml.Emitter;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Buffers.Text;
using System.Globalization;
using System.Text;

namespace NexVYaml;

public class YamlWriter : IYamlWriter
{
    Utf8YamlEmitter Emitter { get; set; }
    YamlSerializationContext SerializeContext { get; init; }
    internal YamlWriter(Utf8YamlEmitter emitter, YamlSerializationContext context)
    {
        SerializeContext = context;
        Emitter = emitter;
    }
    public void Serialize<T>(ref T value, DataStyle style = DataStyle.Any)
    {
        if (value is null)
        {
            ReadOnlySpan<byte> buf = YamlCodes.Null0;
            Serialize(buf);
            return;
        }
        if (value is Array)
        {
            var t = typeof(T).GetElementType()!;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Serialize(this, value, style);
            return;
        }
        SerializeContext.Serialize(this, value, style);
    }
    public void BeginMapping(DataStyle style)
    {
        Emitter.BeginMapping(style);
    }

    public void BeginSequence(DataStyle style)
    {
        Emitter.BeginSequence(style);
    }

    public void EndMapping()
    {
        Emitter.EndMapping();
    }

    public void EndSequence()
    {
        Emitter.EndSequence();
    }

    /// <summary>
    /// Serializes the specified value as the suggested <see cref="ScalarStyle"/> of <see cref="EmitStringAnalyzer.Analyze(string)"/> string.
    /// </summary>
    /// <param name="value">The value to be Serialized to the Stream.</param>
    public void Serialize(ref string? value)
    {
        var result = EmitStringAnalyzer.Analyze(value);
        var style = result.SuggestScalarStyle();
        if(style is ScalarStyle.Plain or ScalarStyle.Any)
        {
            Span<byte> span = stackalloc byte[value.Length];
            StringEncoding.Utf8.GetBytes(value, span);
            Serialize(span);
        }
        else if(ScalarStyle.Folded == style)
        {
            throw new NotSupportedException($"The {ScalarStyle.Folded} is not supported.");
        }
        else if(ScalarStyle.SingleQuoted == style)
        {
            throw new InvalidOperationException("Single Quote is reserved for char");
        }
        else if(ScalarStyle.DoubleQuoted == style)
        {
            var scalarStringBuilt = EmitStringAnalyzer.BuildQuotedScalar(value, true);
            var stringConverted = scalarStringBuilt.ToString();
            Span<byte> span = stackalloc byte[stringConverted.Length];
            StringEncoding.Utf8.GetBytes(stringConverted, span);
            Serialize(span);
        }
        else if(ScalarStyle.Literal == style)
        {
            var indentCharCount = (Emitter.CurrentIndentLevel + 1) * Utf8YamlEmitter.IndentWidth;
            var scalarStringBuilt = EmitStringAnalyzer.BuildLiteralScalar(value, indentCharCount);
            Span<char> scalarChars = stackalloc char[scalarStringBuilt.Length];
            scalarStringBuilt.CopyTo(0, scalarChars, scalarStringBuilt.Length);

            scalarChars = TryRemoveDuplicateLineBreak(Emitter, scalarChars);

            var maxByteCount = StringEncoding.Utf8.GetMaxByteCount(scalarChars.Length);
            var offset = 0;
            var output = Emitter.Writer.GetSpan(Emitter.CalculateMaxScalarBufferLength(maxByteCount));
            Emitter.BeginScalar(output, ref offset);
            offset += StringEncoding.Utf8.GetBytes(scalarChars, output[offset..]);
            Emitter.EndScalar(output, ref offset);
        }
    }
    private static Span<char> TryRemoveDuplicateLineBreak(Utf8YamlEmitter emitter, Span<char> scalarChars)
    {
        if (emitter.StateStack.Current is EmitState.BlockMappingValue or EmitState.BlockSequenceEntry)
        {
            scalarChars = scalarChars[..^1];
        }

        return scalarChars;
    }

    public void Serialize(ReadOnlySpan<byte> value)
    {
        Emitter.WriteScalar(value);
    }

    public void WriteTag(string tag)
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
