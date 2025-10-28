using NexYaml.Parser;
using NexYaml.Serialization;
namespace NexYaml.Parser;
public record ScopeContext(YamlReader Reader, IYamlSerializerResolver Resolver, IdentifiableResolver IdentifiableResolver);
