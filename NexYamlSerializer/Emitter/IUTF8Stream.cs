using NexYamlSerializer.Emitter;
using NexYamlSerializer.Emitter.Serializers;
using Stride.Core;
using System;
using System.Buffers;

namespace NexVYaml.Emitter;
public interface IUTF8Stream : IDisposable
{
    int CurrentIndentLevel { get; }
    ArrayBufferWriter<byte> Writer { get; }
    IEmitterFactory EmitterFactory { get; }
    IEmitter Current { get; set; }
    IEmitter Previous { get; }
    IEmitter Next { set; }
    void Tag(ref string value);
    void WriteScalar(ReadOnlySpan<byte> value);
    void BeginScalar(Span<byte> output);
    void EndScalar();
    int CalculateMaxScalarBufferLength(int length);
    void Reset();
    Span<char> TryRemoveDuplicateLineBreak(Span<char> scalarChars);
}