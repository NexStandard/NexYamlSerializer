using Stride.Core;

namespace NexYaml.Serialization;

/// <summary>
/// Base class for all YAML nodes.
/// Provides methods for beginning <see cref="Mapping"/> and <see cref="Sequence"/>.
/// </summary>
public abstract class Node
{
    /// <summary>
    /// Begins a new <see cref="Mapping"/> node.
    /// </summary>
    /// <typeparam name="T">The parent <see cref="Node"/> type.</typeparam>
    /// <param name="context">The current serialization <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The YAML tag associated with this <see cref="Mapping"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/></param>
    /// <returns>The next <see cref="WriteContext{Mapping}"/> for the upcomming <see cref="Mapping"/>.</returns>
    public abstract WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style) where T : Node;

    /// <summary>
    /// Begins a new <see cref="Sequence"/> node.
    /// </summary>
    /// <typeparam name="T">The parent <see cref="Node"/> type.</typeparam>
    /// <param name="context">The current serialization <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The YAML tag associated with this <see cref="Sequence"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/></param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> for the upcomming <see cref="Sequence"/>.</returns>
    public abstract WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style) where T : Node;

    /// <summary>
    /// Ends the current <see cref="Node"/> context, preventing further traversal deeper into the <see cref="Node"/> tree.
    /// This effectively causes the context to bubble up to the active parent <see cref="Node"/>.
    /// While this process can occur implicitly, some <see cref="Node"/> types may require an explicit ending.
    /// </summary>
    /// <typeparam name="T">The node type.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/> to be finalized.</param>
    public virtual void End<T>(WriteContext<T> context) where T : Node
    {
        // standard do nothing
    }
}
