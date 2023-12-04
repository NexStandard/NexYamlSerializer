#nullable enable
using VYaml.Emitter;

namespace VYaml.Serialization
{
    public class YamlSerializerOptions
    {
        public static YamlSerializerOptions Standard => new()
        {
            Resolver = NexYamlSerializerRegistry.Instance
        };

        public IYamlFormatterResolver Resolver { get; set; } = null!;
        public YamlEmitOptions EmitOptions { get; set; } = new();
        public bool EnableAliasForDeserialization { get; set; } = true;
    }
}
