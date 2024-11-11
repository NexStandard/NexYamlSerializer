using NexYaml.Core;
using NexYaml.Serialization.Emittters;
using System;

namespace NexYaml.Serialization;
public interface IUTF8Stream : IDisposable
{
    int CurrentIndentLevel { get; }
    int IndentWidth { get; }
    IEmitterFactory EmitterFactory { get; }
    IEmitter Current { get; set; }
    IEmitter Previous { get; }
    IEmitter Next { set; }
    IUTF8Stream Tag(ref string value);
    IUTF8Stream WriteScalar(ReadOnlySpan<byte> value);
    IUTF8Stream WriteScalar(ReadOnlySpan<char> value);
    IUTF8Stream WriteScalar(string value);
    IUTF8Stream WriteRaw(ReadOnlySpan<byte> value);
    IUTF8Stream WriteRaw(ReadOnlySpan<char> value);
    IUTF8Stream WriteRaw(string value);
    IUTF8Stream WriteIndent(int forceWidth = -1);
    ReadOnlyMemory<char> GetChars();
    public SyntaxSettings settings { get; }
    void Reset();
    Span<char> TryRemoveDuplicateLineBreak(Span<char> scalarChars);
}