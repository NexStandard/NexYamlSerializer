using NexYaml.Core;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml;
/// <summary>
/// Defines methods for writing YAML data, including support for various data structures,
/// type resolution, and formatting styles. This interface provides a flexible serialization 
/// mechanism to support different YAML syntax settings and reference caching during serialization.
/// </summary>
public interface IYamlWriter
{

    public bool IsRedirected { get; set; }
    /// <summary>
    /// Writes lazy a YAML tag for the object type, depending on its serialization context.
    /// </summary>
    /// <param name="tag">The YAML tag identifying the object's type.</param>
    void WriteTag(string tag, bool force =false);

    /// <summary>
    /// Gets the settings that define YAML syntax and formatting behavior.
    /// </summary>
    SyntaxSettings Settings { get; }

    /// <summary>
    /// Maintains a cache of references used during serialization to prevent duplication.
    /// </summary>
    HashSet<Guid> References { get; }

    /// <summary>
    /// Provides type resolution for all serializable types.
    /// </summary>
    IYamlSerializerResolver Resolver { get; }

    /// <summary>
    /// Begins a YAML sequence formatted according to the specified style.
    /// Call <see cref="EndSequence"/> to close the sequence.
    /// </summary>
    /// <param name="style">Defines the style used to format the sequence.</param>
    void BeginSequence(DataStyle style);

    /// <summary>
    /// Ends the YAML sequence initiated by <see cref="BeginSequence(DataStyle)"/>.
    /// </summary>
    void EndSequence();

    /// <summary>
    /// Begins a YAML mapping formatted according to the specified style.
    /// Call <see cref="EndMapping"/> to close the mapping.
    /// </summary>
    /// <param name="style">Defines the style used to format the mapping.</param>
    void BeginMapping(DataStyle style);

    /// <summary>
    /// Ends the YAML mapping initiated by <see cref="BeginMapping(DataStyle)"/>.
    /// </summary>
    void EndMapping();

    /// <summary>
    /// Writes the specified value of type <typeparamref name="T"/> using the provided style.
    /// The <see cref="Resolver"/> is used to determine the appropriate serializer for <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to write to the output.</param>
    /// <param name="style">Defines the formatting style for the value.</param>
    void WriteType<T>(T value, DataStyle style);

    /// <summary>
    /// Writes a scalar value to the output from a span of bytes, formatted according to syntax settings.
    /// </summary>
    /// <param name="bytes">The bytes to write as a scalar value.</param>
    void WriteScalar(ReadOnlySpan<byte> bytes);

    /// <summary>
    /// Writes a scalar value to the output from a span of characters, formatted according to syntax settings.
    /// </summary>
    /// <param name="chars">The characters to write as a scalar value.</param>
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

    public void WriteRaw(ReadOnlySpan<char> value);
}