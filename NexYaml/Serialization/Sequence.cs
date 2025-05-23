using Stride.Core;

namespace NexYaml.Serialization;

/// <summary>
/// Represents a YAML <see cref="Sequence"/> node.
/// Allows writing ordered elements.
/// </summary>
public abstract class Sequence : Node
{
    /// <summary>
    /// Writes an element into the <see cref="Sequence"/> node.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="context">The current <see cref="Sequence"/> <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to add to the sequence.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> for the <see cref="Sequence"/>.</returns>
    public abstract WriteContext<Sequence> Write<T>(WriteContext<Sequence> context, T value, DataStyle style);
}
