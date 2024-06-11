using NexVYaml.Serialization;
using NexYaml.Core;
using NexYamlSerializer.NewYaml;
using Stride.Core;
using System;
using System.Linq;

namespace NexVYaml;
public interface IYamlWriter : IYamlStream
{
    public void WriteTag(string tag);
    public void BeginMapping(DataStyle style);
    public void EndMapping();
    public void BeginSequence(DataStyle style);
    public void EndSequence();
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
    public void Serialize<T>(ref T value, DataStyle style);
}