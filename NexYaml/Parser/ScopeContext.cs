using NexYaml.Parser;
using NexYaml.Serialization;

public record ScopeContext(YamlReader Reader, IYamlSerializerResolver Resolver, IdentifiableResolver IdentifiableResolver);
