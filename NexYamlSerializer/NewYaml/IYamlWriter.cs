using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Linq;

namespace NexVYaml;
public interface IYamlWriter
{
    /// <summary>
    /// Writes lazy the Yaml Tag to the stream depending on the context of what will be written for that object.
    /// </summary>
    /// <param name="tag">The Tag of the class to identify its type</param>
    void WriteTag(string tag);
    void BeginMapping(DataStyle style);
    void EndMapping();
    void BeginSequence(DataStyle style);
    void EndSequence();
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
    void Serialize(ref string? value);
    void Serialize(ReadOnlySpan<byte> value);
    void Serialize<T>(ref T value, DataStyle style);
}