using NexYaml.Serialization;

namespace NexYaml;
/// <summary>
/// Defines the factory interface for creating YAML serializers. 
/// It allows registering a resolver and instantiating specific YAML serializers for given types.
/// </summary>
public interface IYamlSerializerFactory
{
    /// <summary>
    /// Registers the provided <see cref="IYamlSerializerResolver"/> with the factory.
    /// The resolver is used to manage and provide the necessary serializers for different types.
    /// </summary>
    /// <param name="resolver">The <see cref="IYamlSerializerResolver"/> to be registered.</param>
    void Register(IYamlSerializerResolver resolver);

    /// <summary>
    /// Instantiates a YAML serializer for the specified target type.
    /// The serializer is responsible for serializing and deserializing objects of the given type.
    /// </summary>
    /// <param name="target">The target type for which a serializer is to be instantiated.</param>
    /// <returns>A <see cref="YamlSerializer"/> instance for the specified type.</returns>
    public YamlSerializer Instantiate(Type target);
}
