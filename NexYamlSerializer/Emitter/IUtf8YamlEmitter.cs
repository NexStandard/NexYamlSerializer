using Stride.Core;
using System;
using System.Buffers;

namespace NexVYaml.Emitter;
public interface IUTF8Emitter
{
    int CurrentIndentLevel { get; }
    YamlEmitOptions Options { get; }
    IBufferWriter<byte> Writer { get; }

    int CalculateMaxScalarBufferLength(int length);
    void BeginMapping(DataStyle style = DataStyle.Normal);
    void EndMapping();
    void BeginSequence(DataStyle style = DataStyle.Normal);
    void EndSequence();
    void Tag(string value);
    void BeginScalar(Span<byte> output, ref int offset);
    void EndScalar(Span<byte> output, ref int offset);
    void WriteScalar(ReadOnlySpan<byte> value);
}
