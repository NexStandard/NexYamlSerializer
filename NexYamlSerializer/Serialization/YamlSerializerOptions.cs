#nullable enable
using NexVYaml.Emitter;

namespace NexVYaml.Serialization
{
    public class YamlSerializerOptions
    {
        public static YamlSerializerOptions Standard => new();

        public IYamlFormatterResolver Resolver { get; set; } = NexYamlSerializerRegistry.Instance;
        public YamlEmitOptions EmitOptions { get; set; } = new();
        /// <summary>
        /// Limits <see cref="RedirectFormatter{T}"/> Serialization/Deserialization to known Basic Formatters.
        /// Tags won't have an effect but will be still written to the file.
        /// Interface/Abstract Serialization/Deserialization is blocked.
        /// </summary>
        public bool SecureMode { get; set; } = false;
    }
}
