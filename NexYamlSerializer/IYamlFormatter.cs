#nullable enable
using NexVYaml.Parser;
using NexVYaml.Serialization;
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
}