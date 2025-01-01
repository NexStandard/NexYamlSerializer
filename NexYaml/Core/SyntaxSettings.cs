namespace NexYaml.Core;

public class SyntaxSettings()
{
    public char KeyValueSeparator { get; init; } = ':';
    public string Null { get; init; } = "!!null";
    public string Reference { get; init; } = "!!ref";
    public string SequenceIdentifier { get; init; } = "- ";
    public string FlowMappingStart { get; init; } = "{";
    public string FlowMappingEnd { get; init; } = "}";
    public string FlowSequenceStart { get; init; } = "[";
    public string FlowSequenceEnd { get; init; } = "]";
    public string FlowMappingDelimiter { get; init; } = ",";
    public string FlowSequenceSeparator { get; init; } = ",";
    public string True { get; init; } = "true";
    public string False { get; init; } = "false";
}
