using Stride.Core;

namespace NexYaml.Serialization;
/// <summary>
/// Represents a YAML <see cref="Mapping"/> node.
/// Allows writing key-value pairs.
/// </summary>
public abstract class Mapping : Node
{
    /// <summary>
    /// Writes a key-value pair into the <see cref="Mapping"/> node.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="key">The key for the <see cref="Mapping"/> entry.</param>
    /// <param name="value">The value associated with the <paramref name="key"/>.</param>
    /// <param name="context">The current <see cref="Mapping"/> context.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>A new <see cref="WriteContext{T}"/> for the next upcomming <see cref="Node"/>.</returns>
    public abstract WriteContext<Mapping> Write<T>(WriteContext<Mapping> context, string key, T value, DataStyle style);
}
