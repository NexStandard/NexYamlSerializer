#nullable enable
using NexVYaml.Emitter;

namespace NexVYaml.Serialization
{
    public class YamlSerializerOptions
    {
        public static YamlSerializerOptions Standard => new()
        {
            Resolver = NexYamlSerializerRegistry.Instance
        };

        public IYamlFormatterResolver Resolver { get; set; } = null!;
        public YamlEmitOptions EmitOptions { get; set; } = new();
        /// <summary>
        /// Experimental:
        /// Uses <see cref="SecureRedirectFormatter{T}"/>.
        /// Limits Serialization/Deserialization to known Formatters, which are non generic.
        /// Tags won't have an effect.
        /// </summary>
        public bool SecureMode { get; set; } = false;
        public bool EnableAliasForDeserialization { get; set; } = true;
    }
}
