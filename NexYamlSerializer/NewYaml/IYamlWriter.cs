using NexVYaml.Serialization;
using NexYaml.Core;
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
    void Write(string? value, DataStyle style = DataStyle.Any);
    void Write(ReadOnlySpan<byte> value, DataStyle style = DataStyle.Any);
    void Write(char value, DataStyle style = DataStyle.Any);
    void Write(int value, DataStyle style = DataStyle.Any);
    void Write(uint value, DataStyle style = DataStyle.Any);
    void Write(long value, DataStyle style = DataStyle.Any);
    void Write(ulong value, DataStyle style = DataStyle.Any);
    void Write(float value, DataStyle style = DataStyle.Any);
    void Write(double value, DataStyle style = DataStyle.Any);
    void Write(bool value, DataStyle style = DataStyle.Any);
    void Write(short value, DataStyle style = DataStyle.Any);
    void Write(ushort value, DataStyle style = DataStyle.Any);
    void Write(byte value, DataStyle style = DataStyle.Any);
    void Write(sbyte value, DataStyle style = DataStyle.Any);
    void Write(decimal value, DataStyle style = DataStyle.Any);
    void Write<T>(T value, DataStyle style);
}