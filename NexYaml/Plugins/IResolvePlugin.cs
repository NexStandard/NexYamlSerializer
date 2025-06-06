﻿using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;

/// <summary>
/// Represents a plugin interface for resolving YAML serialization and deserialization logic.
/// Implementers provide custom processing for reading and writing YAML structures.
/// </summary>
public interface IResolvePlugin
{
    /// <summary>
    /// Writes a serialized representation of the provided value into the given write context.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/> being written.</typeparam>
    /// <typeparam name="X">The type of the <see cref="Node"/> that represents the YAML structure.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/></param>
    /// <param name="value">The <paramref name="value"/> to serialize into YAML.</param>
    /// <param name="style">The <see cref="DataStyle"/></param>
    /// <returns><c>true</c> if the value was successfully handled by the <see cref="IResolvePlugin"/>, otherwise <c>false</c>.</returns>
    bool Write<T, X>(WriteContext<X> context, T value, DataStyle style)
        where X : Node;

    /// <summary>
    /// Reads and parses a YAML stream to extract a typed value.
    /// </summary>
    /// <typeparam name="T">The expected type of the deserialized object.</typeparam>
    /// <param name="stream">The YAML reader stream providing input data.</param>
    /// <param name="value">The resulting parsed value, wrapped in a <see cref="ValueTask{T}"/></param>
    /// <param name="result">The <see cref="ParseContext"/></param>
    /// <returns><c>true</c> if the value was successfully parsed, otherwise <c>false</c>.</returns>
    bool Read<T>(IYamlReader stream, out ValueTask<T?> value, ParseContext result);
}