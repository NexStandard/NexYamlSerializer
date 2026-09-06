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
    /// Writes a value using the specified <see cref="DataStyle"/> into the current YAML <see cref="Node"/>.
    /// </summary>
    /// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
    /// <typeparam name="X">The type of the value to write.</typeparam>
    /// <param name="node">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to write to YAML. May be <c>null</c>.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteType<T>(this Node node, T? value, DataStyle style)
    {
        node.Writer.WriteType(node, value, style);
    }

    /// <summary>
    /// Writes a {KEY} : {VALUE} pair into the current YAML <see cref="Mapping"/> <see cref="WriteContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="node">The <see cref="WriteContext{T}"/> as a YAML <see cref="Mapping"/>.</param>
    /// <param name="key">The key for the <see cref="Mapping"/> entry.</param>
    /// <param name="value">The value associated with the key.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The  <see cref="WriteContext{T}"/> based on the written key/value pair.</returns>
    public static Node Write<T>(this Node node, ReadOnlySpan<char> key, T value, DataStyle style = DataStyle.Any)
    {
        node.WriteMap(key, style);
        if (value is null)
        {
            node.WriteScalar(YamlCodes.Null.AsSpan());
            return node;
        }
        node.WriteType(value, style);
        return node;
    }

    /// <summary>
    /// Writes a value as an entry in the current YAML <see cref="Sequence"/> <see cref="WriteContext{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="node">The <see cref="WriteContext{T}"/> representing a YAML <see cref="Sequence"/>.</param>
    /// <param name="value">The value to write as a <see cref="Sequence"/> item.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> based on the written value.</returns>
    public static Node Write<T>(this Node node, T value, DataStyle style = DataStyle.Any)
    {
        node.WriteElement(value, style);
        return node;
    }

    public static Node Write(this Node node, ReadOnlySpan<char> key, string value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        node.WriteMap(key, style);
        if (value is null)
        {
            node.WriteScalar(YamlCodes.Null.AsSpan());
            return node;
        }
        node.WriteScalar(node.Writer.FormatString(node, value, style));
        return node;
    }
    public static Node Write(this Node node, ReadOnlySpan<char> key, Guid value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        node.WriteMap(key, style);
        Span<char> buffer = stackalloc char[36]; // or 32 if you want "N" format

        if (value.TryFormat(buffer, out int written, "D"))
        {
            node.WriteScalar(buffer);
        }
        return node;
    }
}
