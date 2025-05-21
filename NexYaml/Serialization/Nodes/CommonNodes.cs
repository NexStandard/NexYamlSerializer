namespace NexYaml.Serialization.Nodes
{
    internal static class CommonNodes
    {
        public static BlockMapping BlockMapping { get; } = new BlockMapping();
        public static BlockSequence BlockSequence { get; } = new BlockSequence();
        public static FlowMapping FlowMapping { get; } = new FlowMapping();
        public static FlowMappingSecondary FlowMappingSecondary { get; } = new FlowMappingSecondary();
        public static FlowSequence FlowSequence { get; } = new FlowSequence();
        public static FlowSequenceSecondary FlowSequenceSecondary { get; } = new FlowSequenceSecondary();
        public static BlockSequenceMapping BlockSequenceMapping { get; } = new BlockSequenceMapping();
    }
}
