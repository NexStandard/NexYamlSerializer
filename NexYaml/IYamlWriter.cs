using NexYaml.Serialization;
using Stride.Core;

/// <summary>
/// Defines methods for writing YAML data, including support for various data structures,
/// type resolution, and formatting styles. This interface provides a flexible serialization 
/// mechanism to support different YAML syntax settings and reference caching during serialization.
/// </summary>
public interface IYamlWriter
{
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
    /// Begins a YAML sequence, formatted according to the specified style. 
    /// A sequence represents a list of values in YAML.
    /// </summary>
    /// <param name="style">The style to use for formatting the sequence.</param>
    void BeginSequence(string tag, DataStyle style);

    /// <summary>
    /// Ends the YAML sequence that was started by <see cref="BeginSequence(DataStyle)"/>.
    /// </summary>
    void EndSequence();

    /// <summary>
    /// Begins a YAML mapping, formatted according to the specified style. 
    /// A mapping represents a set of key-value pairs in YAML.
    /// </summary>
    /// <param name="style">The style to use for formatting the mapping.</param>
    void BeginMapping(string tag, DataStyle style);

    /// <summary>
    /// Ends the YAML mapping that was started by <see cref="BeginMapping(DataStyle)"/>.
    /// </summary>
    void EndMapping();

    /// <summary>
    /// Writes a value of type <typeparamref name="T"/> using the provided formatting style.
    /// The <see cref="Resolver"/> is used to determine the appropriate serializer for the value.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="style">The style to use for formatting the value.</param>
    void WriteType<T>(T value, DataStyle style);

    /// <summary>
    /// Writes a scalar value from a span of bytes to the output, formatted according to YAML syntax settings.
    /// A scalar represents a single value such as a string, integer, or boolean.
    /// </summary>
    /// <param name="bytes">The bytes representing the scalar value.</param>
    void WriteScalar(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Writes a scalar value from a span of characters to the output, formatted according to YAML syntax settings.
    /// </summary>
    /// <param name="chars">The characters representing the scalar value.</param>
    void WriteScalar(ReadOnlySpan<char> chars);

    /// <summary>
    /// Serializes the string, in an automated format <see cref="ScalarStyle"/>
    /// Formats are:
    /// <see cref="ScalarStyle.Any"/> : <see cref="ScalarStyle.Plain"/>
    /// <see cref="ScalarStyle.DoubleQuoted"/> : If the string has special tokens, but no <see cref="YamlCodes.Lf"/>
    /// <see cref="ScalarStyle.Literal"/> : If the string has special tokens, but a <see cref="YamlCodes.Lf"/>
    /// <see cref="ScalarStyle.Folded"/> : Not Implemented and can be covered with literal
    /// <see cref="ScalarStyle.SingleQuoted"/> : Is reserved for <see cref="char"/>
    /// </summary>
    /// <param name="value">The string to write in an auto detection format to the stream.</param>
    void WriteString(string? value, DataStyle style);

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