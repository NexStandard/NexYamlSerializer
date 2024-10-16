using NexVYaml.Parser;
using Stride.Core;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NexVYaml.Parser;
public interface IYamlReader
{
    bool HasKeyMapping { get; }
    bool HasSequence { get; }

    void Read(ref ReadOnlySpan<byte> span);
    void Read<T>(ref T? value);

    void Dispose();
    bool HasMapping(out ReadOnlySpan<byte> mappingKey);
    bool IsNullScalar();
    bool Move();
    void ReadWithVerify(ParseEventType eventType);
    void Reset();
    void SkipAfter(ParseEventType eventType);
    void SkipRead();
    bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);
    bool TryGetScalarAsString(out string? value);
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey);
}
