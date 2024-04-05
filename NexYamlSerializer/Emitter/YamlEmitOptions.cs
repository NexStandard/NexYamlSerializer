#nullable enable
namespace NexVYaml.Emitter;

public enum SequenceStyle
{
    Block,
    Flow,
}
public enum YamlStyle
{
    BlockMapping,
    FlowMapping,
    BlockSequence,
    FlowSequence
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

