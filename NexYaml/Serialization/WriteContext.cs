using Stride.Core;

namespace NexYaml.Serialization;

/// <summary>
/// The current context for writing YAML content. This record encapsulates the current state during YAML serialization,
/// </summary>
public readonly struct WriteContext<T>(int indent, bool isRedirected, DataStyle styleScope, T node, Writer writer) where T : Node
{
    /// <summary>
    /// The current indentation level for formatting the YAML output.
    /// </summary>
    public int Indent { get; init; } = indent;
    /// <summary>
    /// A flag indicating whether the context has been redirected (i.e., when the actual runtime type differs from the expected type).
    /// </summary>
    public bool IsRedirected { get; init; } = isRedirected;
    /// <summary>
    /// The <see cref="DataStyle"/> (e.g., compact, normal) used for the current <see cref="Node"/>.
    /// </summary>
    public DataStyle StyleScope { get; init; } = styleScope;
    /// <summary>
    /// The active YAML node associated with this context.
    /// </summary>
    public T Node { get; init; } = node;
    /// <summary>
    /// The <see cref="Writer"/> instance that handles the output of the YAML content.
    /// </summary>
    public Writer Writer { get; init; } = writer;
}
