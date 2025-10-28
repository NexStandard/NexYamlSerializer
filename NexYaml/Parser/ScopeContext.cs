using NexYaml.Serialization;
namespace NexYaml.Parser;
/// <summary>
/// Encapsulates the shared state required while parsing YAML scopes.
/// </summary>
/// <param name="Reader">
/// The <see cref="YamlReader"/> providing line‑by‑line access to the YAML input.
/// </param>
/// <param name="Resolver">
/// The <see cref="IYamlSerializerResolver"/> used to construct appropriate .NET representations
/// for parsed YAML nodes.
/// </param>
/// <param name="IdentifiableResolver">
/// A resolver that resoves !!ref scalars
/// </param>
public record ScopeContext(
    YamlReader Reader,
    IYamlSerializerResolver Resolver,
    IdentifiableResolver IdentifiableResolver
);

