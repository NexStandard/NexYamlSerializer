namespace NexYaml.Serialization;

/// <summary>
/// The <see cref="IYamlSerializerResolver"/> interface defines the contract for resolving and managing serializers
/// for different types It allows for retrieving, registering, and resolving serializers
/// based on types and aliases, supporting both generic and non-generic serializers.
/// </summary>
public interface IYamlSerializerResolver
{
    /// <summary>
    /// Retrieves the serializer for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <returns>The <see cref="IYamlSerializer{T}"/> for the specified type.</returns>
    IYamlSerializer<T> GetSerializer<T>();

    /// <summary>
    /// Retrieves the serializer for the specified type and its origin, 
    /// which is useful for resolving inheritance or interfaces.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> of the object to serialize.</param>
    /// <param name="origin">The <see cref="Type"/> representing the original type or base type.</param>
    /// <returns>The <see cref="IYamlSerializer"/> for the specified type and origin.</returns>
    IYamlSerializer GetSerializer(Type type, Type origin);

    /// <summary>
    /// Retrieves the alias type associated with the specified alias.
    /// </summary>
    /// <param name="alias">The alias string.</param>
    /// <returns>The <see cref="Type"/> associated with the alias.</returns>
    public Type GetAliasType(string alias);

    /// <summary>
    /// Registers a custom YAML serializer factory for a given target type and interface type.
    /// This allows the resolver to generate serializers dynamically for a specific combination of types.
    /// </summary>
    /// <param name="yamlFactory">The custom YAML serializer factory.</param>
    /// <param name="target">The <see cref="Type"/> of the target object to serialize.</param>
    /// <param name="interfaceType">The <see cref="Type"/> of the interface for the target object.</param>
    public void Register(IYamlSerializerFactory yamlFactory, Type target, Type interfaceType);

    /// <summary>
    /// Registers a specific serializer for the given type.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="serializer">The <see cref="IYamlSerializer{T}"/> instance to register.</param>
    public void RegisterSerializer<T>(IYamlSerializer<T> serializer);

    /// <summary>
    /// Registers a specific serializer for a given type.
    /// </summary>
    /// <param name="serializer">The <see cref="IYamlSerializer"/> instance to register.</param>
    public void RegisterSerializer(Type serializer);

    /// <summary>
    /// Registers a tag for a specific serializer type.
    /// This allows the use of a custom tag to represent a specific type during serialization.
    /// </summary>
    /// <param name="tag">The tag string to associate with the serializer.</param>
    /// <param name="serializerType">The <see cref="Type"/> of the serializer to associate with the tag.</param>
    public void RegisterTag(string tag, Type serializerType);

    /// <summary>
    /// Registers a generic serializer for a specific target type and serializer type.
    /// This allows the resolver to handle generic serialization for a target type.
    /// </summary>
    /// <param name="target">The <see cref="Type"/> of the target object to serialize.</param>
    /// <param name="serializerType">The <see cref="Type"/> of the serializer to handle generic serialization.</param>
    public void RegisterGenericSerializer(Type target, Type serializerType);

    /// <summary>
    /// The default implementation of the <see cref="IYamlSerializerResolver"/> interface.
    /// This is typically used when no custom resolver is needed, providing default behavior.
    /// </summary>
    public static IYamlSerializerResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
}
