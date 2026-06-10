using NexYaml.Core;
using Stride.Core;

namespace NexYaml.Serialization;
/// <summary>
/// Represents a YAML <see cref="Mapping"/> node.
/// Allows writing key-value pairs.
/// </summary>
public abstract class Mapping : Node
{
    public Mapping(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
    : base(indent, isRedirected, styleScope, writer)
    {
    }
    /// <summary>
    /// Writes a key-value pair into the <see cref="Mapping"/> node.
    /// </summary>
    /// <typeparam name="T">The type of the <paramref name="value"/>.</typeparam>
    /// <param name="context">The current <see cref="Mapping"/> context.</param>
    /// <param name="key">The key for the <see cref="Mapping"/> entry.</param>
    /// <param name="value">The value associated with the <paramref name="key"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    /// <returns>The next <see cref="WriteContext{Mapping}"/> for the <see cref="Mapping"/>.</returns>
    public virtual Mapping Writes<T>(string key, T value, DataStyle style)
    {
        if (value is null)
        {
            WriteScalar(YamlCodes.Null.AsSpan());
            return this;
        }
        this.WriteType(value, style);
        return this;
    }
    public abstract Mapping Begin(Mapping context, string key, DataStyle style);
    public abstract Mapping End(Mapping context, DataStyle style);
}
