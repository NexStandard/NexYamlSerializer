using System.Globalization;
using NexYaml.Core;
using Stride.Core;

namespace NexYaml.Serialization;

/// <summary>
/// Contains extension methods that enhance the functionality of <see cref="WriteContext{T}"/> for YAML serialization operations.
/// </summary>
public static class WriteContextExtensions
{
    /// <summary>
    /// Writes the provided formatted and escaped text to the underlying output.
    /// </summary>
    /// <typeparam name="T">Type of the current <see cref="Node"/></typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="text">A <see cref="ReadOnlySpan{T}"/> of characters representing the formatted text to write.</param>
    public static void WriteScalar<T>(this WriteContext<T> context, ReadOnlySpan<char> text) where T : Node
    {
        context.Writer.Write(text);
    }

    /// <summary>
    /// Begins a new YAML <see cref="Mapping"/> on the current <see cref="Node"/> context using the specified tag and style.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to associate with the new <see cref="Mapping"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/> to use for the new <see cref="Mapping"/>.</param>
    /// <returns><see cref="WriteContext{Mapping}"/> as the started <see cref="Mapping"/>.</returns>
    public static WriteContext<Mapping> BeginMapping<T>(this WriteContext<T> context, string tag, DataStyle style)
        where T : Node
    {
        return context.Node.BeginMapping(context, tag, style);
    }

    /// <summary>
    /// Begins a new YAML <see cref="Sequence"/> on the current <see cref="Node"/> context using the specified tag and style.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="node">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to associate with the new <see cref="Sequence"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/> to use for the new <see cref="Sequence"/>.</param>
    /// <returns><see cref="WriteContext{Mapping}"/> as the started <see cref="Sequence"/>.</returns>
    public static WriteContext<Sequence> BeginSequence<T>(this WriteContext<T> node, string tag, DataStyle style)
        where T : Node
    {
        return node.Node.BeginSequence(node, tag, style);
    }

    /// <summary>
    /// Writes a value using the specified <see cref="DataStyle"/> into the current YAML <see cref="Node"/>.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <typeparam name="X">The type of the value to write.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to write to YAML. May be <c>null</c>.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteType<T, X>(this WriteContext<T> context, X? value, DataStyle style)
        where T : Node
    {
        context.Writer.WriteType(context, value, style);
    }

    /// <summary>
    /// Ends the current YAML <see cref="WriteContext{T}"/>, closing the <see cref="Node"/> that was previously started.
    /// </summary>
    /// <typeparam name="T">The type of the parent YAML <see cref="Node"/>.</typeparam>
    /// <typeparam name="X">The type of the current YAML <see cref="Node"/> that is ending.</typeparam>
    /// <param name="context">The parent <see cref="WriteContext{T}"/>.</param>
    /// <param name="current">The <see cref="WriteContext{T}"/> of the <see cref="Node"/> that is ending.</param>
    public static void End<T, X>(this WriteContext<T> context, in WriteContext<X> current)
        where T : Node
        where X : Node
    {
        context.Node.End(current);
    }

    /// <summary>
    /// Writes an empty <see cref="Mapping"/> with the given tag.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to prefix the empty <see cref="Sequence"/>.</param>
    public static void WriteEmptyMapping<T>(this WriteContext<T> context, string tag)
        where T : Node
    {
        context.WriteScalar(tag);
        context.WriteScalar(" { }");
    }

    /// <summary>
    /// Writes an empty <see cref="Sequence"/> with the given tag.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The tag to prefix the empty sequence.</param>
    public static void WriteEmptySequence<T>(this WriteContext<T> context, string tag)
        where T : Node
    {
        context.WriteScalar(tag);
        context.WriteScalar(" [ ]");
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
    public static WriteContext<Mapping> Write<T>(this WriteContext<Mapping> mapping, string key, T value, DataStyle style = DataStyle.Any)
    {
        return mapping.Node.Write(mapping, key, value, style);
    }

    /// <summary>
    /// Writes a value as an entry in the current YAML <see cref="Sequence"/> <see cref="WriteContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="sequence">The <see cref="WriteContext{T}"/> representing a YAML <see cref="Sequence"/>.</param>
    /// <param name="value">The value to write as a <see cref="Sequence"/> item.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> based on the written value.</returns>
    public static WriteContext<Sequence> Write<T>(this WriteContext<Sequence> sequence, T value, DataStyle style = DataStyle.Any)
    {
        return sequence.Node.Write(sequence, value, style);
    }

    /// <summary>
    /// Writes the provided text to the underlying output with formatting.
    /// </summary>
    /// <typeparam name="X">Type of the current <see cref="Node"/></typeparam>
    /// <param name="value">A <see cref="ReadOnlySpan{T}"/> of characters representing the formatted text to write.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteString<X>(this WriteContext<X> context, string value, DataStyle style = DataStyle.Compact)
        where X : Node
    {
        context.WriteScalar(context.Writer.FormatString(context, value, style));
    }
}
public static class ScalarExtensions
{
    public static WriteContext<Mapping> Write(this WriteContext<Mapping> mapping, string key, int value, DataStyle style = DataStyle.Any)
    {
        Span<char> span = stackalloc char[11];
        value.TryFormat(span, out var written, default, CultureInfo.InvariantCulture);
        return mapping.Node.Write(mapping, key, span[..written], style);
    }

    public static WriteContext<Mapping> Write(this WriteContext<Mapping> mapping, string key, string? value, DataStyle style = DataStyle.Any)
    {
        if (value is null)
        {
            return mapping.Node.Write(mapping, key, YamlCodes.Null.AsSpan(), style);
        }
        return mapping.Node.Write(mapping, key, mapping.Writer.FormatString(mapping, value, style), style);
    }
}
