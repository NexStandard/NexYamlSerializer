using Stride.Core;

namespace NexYaml.Serialization;

/// <summary>
/// The current context for writing YAML content. This record encapsulates the current state during YAML serialization,
/// </summary>
/// <typeparam name="T">The type of the current YAML <see cref="Node"/>.</typeparam>
/// <param name="Indent">The current indentation level for formatting the YAML output.</param>
/// <param name="IsRedirected">A flag indicating whether the context has been redirected (i.e., when the actual runtime type differs from the expected type).</param>
/// <param name="StyleScope">The <see cref="DataStyle"/> (e.g., compact, normal) used for the current <see cref="Node"/>.</param>
/// <param name="Node">The active YAML node associated with this context.</param>
/// <param name="Writer">The <see cref="Writer"/> instance that handles the output of the YAML content.</param>
public record WriteContext<T>(int Indent, bool IsRedirected, DataStyle StyleScope, T Node, Writer Writer) where T : Node;
