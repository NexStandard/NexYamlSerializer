using NexYaml.Plugins;
using NexYaml.Serialization;
using Stride.Core;

/// <summary>
/// Defines methods for writing YAML data, including support for various data structures,
/// type resolution, and formatting styles. This interface provides a flexible serialization 
/// mechanism to support different YAML syntax settings and reference caching during serialization.
/// </summary>
public interface IYamlWriter
{
    public List<IResolvePlugin> Plugins { get; }
    /// <summary>
    /// Maintains a cache of references used during serialization to prevent duplication 
    /// of the same object or value within the YAML data.
    /// </summary>
    HashSet<Guid> References { get; }

    /// <summary>
    /// Provides type resolution for all serializable types through a <see cref="IYamlSerializerResolver"/>.
    /// This resolver helps in managing and providing the appropriate serializer for various types.
    /// </summary>
    IYamlSerializerResolver Resolver { get; }

    /// <summary>
    /// Writes raw characters to the output stream.
    /// </summary>
    /// <param name="value">The raw characters to write.</param>
    public void WriteRaw(ReadOnlySpan<char> value);

    /// <summary>
    /// Writes raw string data to the output stream.
    /// </summary>
    /// <param name="value">The raw string to write.</param>
    public void WriteRaw(string value);

    /// <summary>
    /// Writes a single raw character to the output stream.
    /// </summary>
    /// <param name="value">The raw character to write.</param>
    public void WriteRaw(char value);
}