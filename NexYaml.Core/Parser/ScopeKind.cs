namespace NexYaml.Parser.Scopes;

public enum ScopeKind
{
    /// <summary>
    /// A scalar value (string, number, boolean, or null).
    /// </summary>
    Scalar,

    /// <summary>
    /// A mapping (key–value pairs, equivalent to a dictionary/object).
    /// </summary>
    BlockMapping,

    /// <summary>
    /// A sequence (an ordered list of items).
    /// </summary>
    BlockSequence,
    FlowSequence,
    FlowMapping,
    PrefixedBlockMapping,
    LazyScalar,
    NullScalar
}
