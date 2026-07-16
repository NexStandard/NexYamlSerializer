using NexYaml.Core;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Core.Serialization.Nodes;
/// <summary>
/// Represents a YAML <see cref="Mapping"/> node.
/// Allows writing key-value pairs.
/// </summary>
public interface Mapping : Node
{
    public abstract void WriteMap(Mapping context, ReadOnlySpan<char> key, DataStyle style);
}
