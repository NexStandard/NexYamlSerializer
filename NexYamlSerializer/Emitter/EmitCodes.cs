namespace NexVYaml.Emitter;

internal static class EmitCodes
{

    internal static readonly byte[] BlockSequenceEntryHeader = [(byte)'-', (byte)' '];
    internal static readonly byte[] FlowSequenceEntryHeader = [(byte)'[', (byte)' '];
    internal static readonly byte[] FlowSequenceEmpty = [(byte)'[', (byte)']'];
    internal static readonly byte[] FlowSequenceSeparator = [(byte)',', (byte)' '];
    internal static readonly byte[] MappingKeyFooter = [(byte)':', (byte)' '];
    internal static readonly byte[] FlowMappingEmpty = [(byte)'{', (byte)'}'];
    internal static readonly byte[] FlowMappingStart = [(byte)'{', (byte)' '];
    internal static readonly byte[] FlowMappingEnd = [(byte)' ', (byte)'}'];
}