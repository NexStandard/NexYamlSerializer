using NexVYaml.Emitter;
using NexVYaml.Internal;
using System;

namespace NexYaml2.NewYaml;

unsafe ref struct StringWriter(Utf8YamlEmitter emitter)
{
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
