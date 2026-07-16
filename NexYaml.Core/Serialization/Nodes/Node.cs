using NexYaml.Serialization;
using Stride.Core;
using static Stride.Graphics.Buffer;

namespace NexYaml.Core.Serialization.Nodes;

/// <summary>
/// Base class for all YAML nodes.
/// Provides methods for beginning <see cref="Mapping"/> and <see cref="Sequence"/>.
/// </summary>
public interface Node
{
    /// <summary>
    /// The current indentation level for formatting the YAML output.
    /// </summary>
    public int Indent { get; }
    /// <summary>
    /// A flag indicating whether the context has been redirected (i.e., when the actual runtime type differs from the expected type).
    /// </summary>
    public bool IsRedirected { get; set; }
    /// <summary>
    /// The <see cref="DataStyle"/> (e.g., compact, normal) used for the current <see cref="Node"/>.
    /// </summary>
    public DataStyle StyleScope { get; init; }

    /// <summary>
    /// The <see cref="Writer"/> instance that handles the output of the YAML content.
    /// </summary>
    public Writer Writer { get; init; }
    public abstract Mapping BeginMapping(string tag, DataStyle style);

    public abstract Sequence BeginSequence(string tag, DataStyle style);

    /// <summary>
    /// Ends the current <see cref="Node"/> context, preventing further traversal deeper into the <see cref="Node"/> tree.
    /// This effectively causes the context to bubble up to the active parent <see cref="Node"/>.
    /// While this process can occur implicitly, some <see cref="Node"/> types may require an explicit ending.
    /// </summary>
    public void End();
    public void WriteScalar(ReadOnlySpan<char> text);

    /// <summary>
    /// Writes an empty <see cref="Mapping"/> with the given tag.
    /// </summary>
    public void WriteEmptyMapping(string tag);

    /// <summary>
    /// Writes an empty <see cref="Sequence"/> with the given tag.
    /// </summary>
    public void WriteEmptySequence(string tag);
    /// <summary>
    /// Writes the provided text to the underlying output with formatting.
    /// </summary>
    public void WriteString(string value, DataStyle style = DataStyle.Compact);
}
