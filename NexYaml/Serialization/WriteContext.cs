using Stride.Core;

namespace NexYaml.Serialization;

public record WriteContext<T>(int Indent, bool IsRedirected, DataStyle StyleScope, T Node, Writer Writer) where T : Node;
