﻿using System.Diagnostics.CodeAnalysis;
using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;

namespace NexYaml;
/// <summary>
/// Defines the interface for a YAML reader that provides methods for parsing and resolving YAML content.
/// </summary>
public interface IYamlReader
{
    /// <summary>
    /// Gets a value indicating whether the current position in the YAML stream has a key mapping.
    /// </summary>
    bool HasKeyMapping { get; }

    /// <summary>
    /// Gets a value indicating whether the current position in the YAML stream is within a sequence.
    /// </summary>
    bool HasSequence { get; }

    /// <summary>
    /// Gets the current marker that represents the reader's position in the YAML stream.
    /// </summary>
    Marker CurrentMarker { get; }

    /// <summary>
    /// Gets the set of identifiable objects that can be resolved during YAML reading.
    /// </summary>
    HashSet<IIdentifiable> Identifiables { get; }

    /// <summary>
    /// Gets the dictionary used for resolving references, where each reference (identified by <see cref="Guid"/>) 
    /// has a list of actions that will resolve the reference when executed.
    /// </summary>
    public Dictionary<Guid, List<Action<object>>> ReferenceResolvingMap { get; }

    /// <summary>
    /// Disposes of any resources used by the YAML reader.
    /// </summary>
    void Dispose();

    /// <summary>
    /// Determines if the current position in the YAML stream has a mapping key, and if so, retrieves it.
    /// </summary>
    /// <param name="mappingKey">The mapping key, if present.</param>
    /// <returns><c>true</c> if there is a mapping key; otherwise, <c>false</c>.</returns>
    bool HasMapping(out ReadOnlySpan<byte> mappingKey);

    bool HasMapping(out byte[] mappingKey, bool proxy);

    /// <summary>
    /// Tries to get the tag currently associated with the YAML stream.
    /// </summary>
    /// <param name="tag">The current tag, if present.</param>
    /// <returns><c>true</c> if a tag is found; otherwise, <c>false</c>.</returns>
    public bool TryGetCurrentTag(out Tag tag);

    /// <summary>
    /// Determines if the current scalar value is null.
    /// </summary>
    /// <returns><c>true</c> if the current scalar value is null; otherwise, <c>false</c>.</returns>
    bool IsNullScalar();

    /// <summary>
    /// Moves the reader to the next scalar value in the YAML stream.
    /// </summary>
    /// <returns><c>false</c> if the end of the stream is reached; otherwise, <c>true</c>.</returns>
    bool Move();

    /// <summary>
    /// Moves the reader to the next scalar value with the expected <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">The expected event type to read.</param>
    void Move(ParseEventType eventType);
    /// <summary>
    /// Resets the reader, clearing any state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Skips the reader to the next occurrence of the specified <paramref name="eventType"/>.
    /// </summary>
    /// <param name="eventType">The event type to skip until.</param>
    void SkipAfter(ParseEventType eventType);

    /// <summary>
    /// Skips the current read operation without storing the result.
    /// </summary>
    void SkipRead();
    public ValueTask<T?> Read<T>(ParseContext parseResult);
    /// <summary>
    /// Attempts to get the current scalar value as a span of bytes.
    /// </summary>
    /// <param name="span">The span of bytes containing the scalar value, if found.</param>
    /// <returns><c>true</c> if the scalar value is successfully retrieved as a span of bytes; otherwise, <c>false</c>.</returns>
    bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span);

    /// <summary>
    /// Attempts to get the current scalar value as a string.
    /// </summary>
    /// <param name="value">The scalar value as a string, if found.</param>
    /// <returns><c>true</c> if the scalar value is successfully retrieved as a string; otherwise, <c>false</c>.</returns>
    bool TryGetScalarAsString(out string? value);

    void RegisterIdentifiable(Guid guid, IIdentifiable identifiable);
    ValueTask<T> AsyncGetRef<T>(Guid guid);
}