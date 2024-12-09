﻿using NexYaml.Parser;
using System.Diagnostics.CodeAnalysis;

namespace NexYaml;
public interface IYamlReader
{
    bool HasKeyMapping { get; }
    bool HasSequence { get; }
    public Dictionary<Guid, Action<object>> ReferenceResolvingMap { get; }
    void Read(ref ReadOnlySpan<byte> span);
    void Read<T>(ref T? value, ref ParseResult parseResult);
    void Read<T>(ref T? value);
    void Dispose();
    bool HasMapping(out ReadOnlySpan<byte> mappingKey);
    void ResolveReferences();
    void AddReference(Guid id, Action<object> resolution);
    public bool TryGetCurrentTag(out Tag tag);
    bool IsNullScalar();
    bool Move();
    void ReadWithVerify(ParseEventType eventType);
    void Reset();
    void SkipAfter(ParseEventType eventType);
    void SkipRead();
    bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);
    bool TryGetScalarAsString(out string? value);
    
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey, ref ParseResult parseResult);
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey);
}
