#nullable enable
namespace NexVYaml.Emitter
{
    public enum ScalarStyle
    {
        Any,
        Plain,
        SingleQuoted,
        DoubleQuoted,
        Literal,
        Folded,
    }

    public enum SequenceStyle
    {
        Block,
        Flow,
    }

    public enum MappingStyle
    {
        Block,
        Flow,
    }

    public class YamlEmitOptions
    {
        public static YamlEmitOptions Default => new();

        public int IndentWidth { get; set; } = 2;
    }
}

