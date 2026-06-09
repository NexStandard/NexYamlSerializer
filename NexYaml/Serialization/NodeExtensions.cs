using System.Globalization;
using NexYaml.Core;
using Stride.Core;
using static System.Net.Mime.MediaTypeNames;

namespace NexYaml.Serialization;

/// <summary>
/// Contains extension methods that enhance the functionality of <see cref="WriteContext{T}"/> for YAML serialization operations.
/// </summary>
public static class NodeExtensions
{
    /// <summary>
    /// Writes the provided formatted and escaped text to the underlying output.
    /// </summary>
    /// <typeparam name="T">Type of the current <see cref="Node"/></typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="text">A <see cref="ReadOnlySpan{T}"/> of characters representing the formatted text to write.</param>
    public static void WriteScalar(this Node context, ReadOnlySpan<char> text)
    {
        context.Writer.Write(text);
    }


    /// <summary>
    /// Writes a value using the specified <see cref="DataStyle"/> into the current YAML <see cref="Node"/>.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <typeparam name="X">The type of the value to write.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to write to YAML. May be <c>null</c>.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteType<T>(this Node context, T? value, DataStyle style)
    {
        context.Writer.WriteType(context, value, style);
    }

    /// <summary>
    /// Writes an empty <see cref="Mapping"/> with the given tag.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to prefix the empty <see cref="Sequence"/>.</param>
    public static void WriteEmptyMapping(this Node context, string tag)
    {
        context.WriteScalar(tag);
        context.WriteScalar(" { }".AsSpan());
    }

    /// <summary>
    /// Writes an empty <see cref="Sequence"/> with the given tag.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to prefix the empty sequence.</param>
    public static void WriteEmptySequence(this Node context, string tag)
    {
        context.WriteScalar(tag);
        context.WriteScalar(" [ ]".AsSpan());
    }

    /// <summary>
    /// Writes a {KEY} : {VALUE} pair into the current YAML <see cref="Mapping"/> <see cref="WriteContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="mapping">The <see cref="WriteContext{T}"/> as a YAML <see cref="Mapping"/>.</param>
    /// <param name="key">The key for the <see cref="Mapping"/> entry.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The  <see cref="WriteContext{T}"/> based on the written key/value pair.</returns>
    public static Mapping Write<T>(this Mapping mapping, string key, T value, DataStyle style = DataStyle.Any)
    {
        var x = mapping.Begin(mapping, key, style);
        x = x.Writes(key, value, style);
        return x.End(x,style);
    }

    /// <summary>
    /// Writes a value as an entry in the current YAML <see cref="Sequence"/> <see cref="WriteContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="sequence">The <see cref="WriteContext{T}"/> representing a YAML <see cref="Sequence"/>.</param>
    /// <param name="value">The value to write as a <see cref="Sequence"/> item.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> based on the written value.</returns>
    public static Sequence Write<T>(this Sequence sequence, T value, DataStyle style = DataStyle.Any)
    {
        return sequence.Write(sequence, value, style);
    }

    /// <summary>
    /// Writes the provided text to the underlying output with formatting.
    /// </summary>
    /// <typeparam name="X">Type of the current <see cref="Node"/></typeparam>
    /// <param name="value">A <see cref="ReadOnlySpan{T}"/> of characters representing the formatted text to write.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteString(this Node context, string value, DataStyle style = DataStyle.Compact)
    {
        context.WriteScalar(context.Writer.FormatString(context, value, style));
    }
    public static Mapping Write(this Mapping context, string key, string value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        var x = context.Begin(context,key, style);
        if (value is null)
        {
            x.WriteScalar(YamlCodes.Null.AsSpan());
            return x.End(x, style);
        }
        x.WriteScalar(context.Writer.FormatString(context, value, style));
        return x.End(x, style);
    }
    public static Mapping Write(this Mapping context, string key, Guid value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        var x = context.Begin(context, key, style);
        context.WriteString(value.ToString());
        return x.End(x, style);
    }
}
