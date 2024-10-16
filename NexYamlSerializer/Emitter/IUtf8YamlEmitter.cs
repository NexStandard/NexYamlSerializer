using Stride.Core;
using System;
using System.Buffers;

namespace NexVYaml.Emitter;
internal interface IUtf8YamlEmitter : IDisposable
{
    int CurrentIndentLevel { get; }
    ArrayBufferWriter<byte> Writer { get; }
    void BeginMapping(DataStyle style);
    void BeginScalar(Span<byte> output, ref int offset);
    void BeginSequence(DataStyle style);
    int CalculateMaxScalarBufferLength(int length);
    void EndMapping();
    void EndScalar(Span<byte> output, ref int offset);
    void EndSequence();
    void Reset();
    void Tag(ref string value);
    void WriteScalar(ReadOnlySpan<byte> value);
    Span<char> TryRemoveDuplicateLineBreak(Span<char> scalarChars);
}