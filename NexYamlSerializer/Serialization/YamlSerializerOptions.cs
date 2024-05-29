#nullable enable
using NexVYaml.Emitter;

namespace NexVYaml.Serialization;

public class YamlSerializerOptions
{
    public static YamlSerializerOptions Standard => new();

    public IYamlFormatterResolver Resolver { get; set; } = IYamlFormatterResolver.Default;
    public YamlEmitOptions EmitOptions { get; set; } = new();
}
