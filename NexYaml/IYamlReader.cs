using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Diagnostics.CodeAnalysis;

namespace NexYaml;
public interface IYamlReader
{
    bool HasKeyMapping { get; }
    bool HasSequence { get; }
    Marker CurrentMarker { get; }
    public HashSet<IIdentifiable> Identifiables { get; }
    public Dictionary<Guid, List<Action<object>>> ReferenceResolvingMap { get; }
    void Read(ref ReadOnlySpan<byte> span);
    void Read<T>(ref T? value, ref ParseResult parseResult);
    void Read<T>(ref T? value);
    void Dispose();
    bool HasMapping(out ReadOnlySpan<byte> mappingKey);
    void ResolveReferences();
    void AddReference(Guid id, Action<object> resolution);
    public bool TryGetCurrentTag(out Tag tag);
    bool IsNullScalar();
    /// <summary>
    /// Moves the Reader to the next Scalar
    /// </summary>
    /// <returns>false if the end of the stream is reached, else true</returns>
    bool Move();
    /// <summary>
    /// Moves the <see cref="IYamlReader"/> to the next <see cref="Scalar"/> with the expected <paramref name="eventType"/>
    /// </summary>
    /// <param name="eventType">The expected <see cref="ParseEventType"/> to be read</param>
    void Move(ParseEventType eventType);
    void Reset();
    /// <summary>
    /// Moves the <see cref="IYamlReader>"/> until the <paramref name="eventType"/> is passed
    /// </summary>
    /// <param name="eventType"></param>
    void SkipAfter(ParseEventType eventType);
    void SkipRead();
    bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);
    bool TryGetScalarAsString(out string? value);
    
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey, ref ParseResult parseResult);
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey);
}
