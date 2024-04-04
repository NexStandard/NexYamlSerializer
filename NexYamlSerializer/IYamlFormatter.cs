#nullable enable
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System;

namespace NexVYaml;

public interface IYamlFormatter
{
    object? IndirectDeserialize(ref YamlParser parser, YamlDeserializationContext context) { return null; }
}
/// <summary>
/// Represents a YAML formatter for objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of objects to be serialized and deserialized.</typeparam>
public interface IYamlFormatter<T> : IYamlFormatter
{
    /// <summary>
    /// Deserializes a YAML representation from the provided <see cref="YamlParser"/> and <see cref="YamlDeserializationContext"/>.
    /// </summary>
    /// <param name="parser">The <see cref="YamlParser"/> containing the YAML data to be deserialized.</param>
    /// <param name="context">The <see cref="YamlDeserializationContext"/> providing deserialization context.</param>
    /// <returns>The deserialized value of type <typeparamref name="T"/>.</returns>
    T? Deserialize(ref YamlParser parser, YamlDeserializationContext context);
}