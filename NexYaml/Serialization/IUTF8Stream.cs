using NexYaml.Core;
using NexYaml.Serialization.Emittters;

namespace NexYaml.Serialization;
public interface IUTF8Stream : IDisposable
{
    IndentationManager IndentationManager { get; }
    int ElementCount { get; set; }
    int CurrentIndentLevel { get; }
    int IndentWidth { get; }
    IEmitterFactory EmitterFactory { get; }
    IEmitter Current { get; set; }
    IEmitter Previous { get; }
    IEmitter Next { set; }
    void Tag(ref string value);
    bool TryGetTag(out string value);
    void WriteScalar(ReadOnlySpan<byte> value);
    void WriteRaw(ReadOnlySpan<byte> value);
    void WriteRaw(byte value);
    ReadOnlyMemory<char> GetChars();
    public SyntaxSettings settings { get; }
    void PopState();
    void Reset();
}