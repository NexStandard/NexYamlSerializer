using Stride.Core;
using static Stride.Graphics.Buffer;

namespace NexYaml.Serialization;

/// <summary>
/// Base class for all YAML nodes.
/// Provides methods for beginning <see cref="Mapping"/> and <see cref="Sequence"/>.
/// </summary>
public abstract class Node(int indent, bool isRedirected, DataStyle styleScope, Writer writer)
{
    /// <summary>
    /// The current indentation level for formatting the YAML output.
    /// </summary>
    public int Indent { get; init; } = indent;
    /// <summary>
    /// A flag indicating whether the context has been redirected (i.e., when the actual runtime type differs from the expected type).
    /// </summary>
    public bool IsRedirected { get; set; } = isRedirected;
    /// <summary>
    /// The <see cref="DataStyle"/> (e.g., compact, normal) used for the current <see cref="Node"/>.
    /// </summary>
    public DataStyle StyleScope { get; init; } = styleScope;

    /// <summary>
    /// The <see cref="Writer"/> instance that handles the output of the YAML content.
    /// </summary>
    public Writer Writer { get; init; } = writer;
    /// <summary>
    /// Begins a new <see cref="Mapping"/> node.
    /// </summary>
    /// <typeparam name="T">The parent <see cref="Node"/> type.</typeparam>
    /// <param name="context">The current serialization <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The YAML tag associated with this <see cref="Mapping"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/></param>
    /// <returns>The next <see cref="WriteContext{Mapping}"/> for the upcomming <see cref="Mapping"/>.</returns>
    public abstract Mapping BeginMapping(string tag, DataStyle style);

    /// <summary>
    /// Begins a new <see cref="Sequence"/> node.
    /// </summary>
    /// <typeparam name="T">The parent <see cref="Node"/> type.</typeparam>
    /// <param name="context">The current serialization <see cref="WriteContext{T}"/>.</param>
    /// <param name="tag">The YAML tag associated with this <see cref="Sequence"/>.</param>
    /// <param name="style">The <see cref="DataStyle"/></param>
    /// <returns>The next <see cref="WriteContext{Sequence}"/> for the upcomming <see cref="Sequence"/>.</returns>
    public abstract Sequence BeginSequence(string tag, DataStyle style);

    /// <summary>
    /// Ends the current <see cref="Node"/> context, preventing further traversal deeper into the <see cref="Node"/> tree.
    /// This effectively causes the context to bubble up to the active parent <see cref="Node"/>.
    /// While this process can occur implicitly, some <see cref="Node"/> types may require an explicit ending.
    /// </summary>
    /// <typeparam name="T">The node type.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/> to be finalized.</param>
    public virtual void End()
    {
        // standard do nothing
    }
}
