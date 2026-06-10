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

    public abstract Mapping WriteKey(Mapping context, string key, DataStyle style);
}
