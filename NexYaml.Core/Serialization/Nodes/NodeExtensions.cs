using System.Globalization;
using NexYaml.Core;
using Stride.Core;
using static System.Net.Mime.MediaTypeNames;

namespace NexYaml.Core.Serialization.Nodes;

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
    /// <param name="context">The current <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to write to YAML. May be <c>null</c>.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public static void WriteType<T>(this Node context, T? value, DataStyle style)
    {
        context.Writer.WriteType(context, value, style);
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
    public static Mapping Write<T>(this Mapping mapping, ReadOnlySpan<char> key, T value, DataStyle style = DataStyle.Any)
    {
        var x = mapping.WriteKey(mapping, key, style);
        if (value is null)
        {
            x.WriteScalar(YamlCodes.Null.AsSpan());
            return x;
        }
        x.WriteType(value, style);
        return x;
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

    public static Mapping Write(this Mapping context, ReadOnlySpan<char> key, string value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        var x = context.WriteKey(context,key, style);
        if (value is null)
        {
            x.WriteScalar(YamlCodes.Null.AsSpan());
            return x;
        }
        x.WriteScalar(context.Writer.FormatString(context, value, style));
        return x;
    }
    public static Mapping Write(this Mapping context, ReadOnlySpan<char> key, Guid value, DataStyle style = DataStyle.Any)
    {
        var Style = style is DataStyle.Any or DataStyle.Normal ? DataStyle.Any : style;

        var x = context.WriteKey(context, key, style);
        Span<char> buffer = stackalloc char[36]; // or 32 if you want "N" format

        if (value.TryFormat(buffer, out int written, "D"))
        {
            context.WriteScalar(buffer);
        }

        
        return x;
    }
}
