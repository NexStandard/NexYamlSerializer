namespace NexYaml.Core;

public static class EmitCodes
{

    public static readonly byte[] BlockSequenceEntryHeader = [(byte)'-', (byte)' '];
    public static readonly byte[] FlowSequenceEntryHeader = [(byte)'[', (byte)' '];
    public static readonly byte[] FlowSequenceEmpty = [(byte)'[', (byte)']'];
    public static readonly byte[] FlowSequenceSeparator = [(byte)',', (byte)' '];
    public static readonly byte[] MappingKeyFooter = [(byte)':', (byte)' '];
    public static readonly byte[] FlowMappingEmpty = [(byte)'{', (byte)'}'];
    public static readonly byte[] FlowMappingStart = [(byte)'{', (byte)' '];
    public static readonly byte[] FlowMappingEnd = [(byte)' ', (byte)'}'];
    public static readonly byte MappingDoubleColon = (byte)':';
    public static readonly byte Space = (byte)' ';
}