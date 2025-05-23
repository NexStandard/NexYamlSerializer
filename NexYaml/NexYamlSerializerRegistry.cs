using System.Reflection;
using NexYaml.Core;
using NexYaml.Serialization;
using NexYaml.Serializers;

namespace NexYaml;
/// <summary>
/// Default Implementation for a <see cref="IYamlSerializerResolver"/> Registry to retrieve the types from.
/// Will be used if no <see cref="IYamlSerializerResolver"/> is given in the <see cref="YamlSerializerOptions"/> when using <see cref="YamlSerializer"/>
/// </summary>
public class NexYamlSerializerRegistry : IYamlSerializerResolver
{
    internal SerializerRegistry SerializerRegistry { get; set; } = new();
    public static NexYamlSerializerRegistry Instance { get; } = new NexYamlSerializerRegistry();

    public Type GetAliasType(string alias)
    {
        return SerializerRegistry.TypeMap[alias];
    }

    public YamlSerializer<T> GetSerializer<T>()
    {
        if (SerializerRegistry.DefinedSerializers.TryGetValue(typeof(T), out var serializer))
        {
            return (YamlSerializer<T>)serializer;
        }
        // search if there is a Factory
        if (SerializerRegistry.SerializerFactory.TryGetValue(typeof(T), out var s2))
        {
            s2.TryGetValue(typeof(T), out var t);
            if (t is not null)
            {
                var tempSerializer = t.Instantiate(typeof(T));
                SerializerRegistry.DefinedSerializers[typeof(T)] = tempSerializer;
                return (YamlSerializer<T>)tempSerializer;
            }
        }
        var s = EmptySerializer<T>.EmptyS();
        SerializerRegistry.DefinedSerializers.Add(typeof(T), s);
        return s;
    }

    public void Register(IYamlSerializerFactory yamlFactory, Type target, Type interfaceType)
    {
        var tar = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
        var inter = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        if (SerializerRegistry.SerializerFactory.TryGetValue(inter, out var factory))
        {
            factory.TryAdd(tar, yamlFactory);
        }
        else
        {
            var dictionary = new Dictionary<Type, IYamlSerializerFactory>(new GenericEqualityComparer());
            SerializerRegistry.SerializerFactory[inter] = dictionary;
            dictionary.TryAdd(target, yamlFactory);
        }
    }

    public YamlSerializer GetSerializer(Type type, Type origin)
    {
        // search if the actual type is in the standard serializers
        if (SerializerRegistry.DefinedSerializers.TryGetValue(type, out var serializer1))
        {
            return serializer1;
        }
        // search if there is a Factory
        if (SerializerRegistry.SerializerFactory.TryGetValue(origin, out var serializer))
        {
            serializer.TryGetValue(type, out var t);
            if (t is not null)
            {
                var tempSerializer = t.Instantiate(origin);
                SerializerRegistry.DefinedSerializers[origin] = tempSerializer;
                return tempSerializer;
            }
        }
        var genericSerializer = typeof(EmptySerializer<>);
        var genericType = genericSerializer.MakeGenericType(origin);
        var emptySerializer = (YamlSerializer?)Activator.CreateInstance(genericType);
        return emptySerializer!;
    }
    static bool s_isReady = false;
#if NET9_0_OR_GREATER
    static readonly Lock s_lock = new();
#elif NET8_0
    static readonly object s_lock = new object();
#endif
    public static IYamlSerializerResolver Create(Assembly assembly)
    {
        var registry = new NexYamlSerializerRegistry
        {
            SerializerRegistry = new()
        };
        var serializerFactories = assembly.GetTypes()
            .Where(t => typeof(IYamlSerializerFactory).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        var standard = typeof(Yaml).Assembly.GetTypes()
            .Where(t => typeof(IYamlSerializerFactory).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        foreach (var serializerFactory in serializerFactories)
        {
            var instance = (IYamlSerializerFactory)Activator.CreateInstance(serializerFactory)!;
            instance.Register(registry);
        }
        foreach (var serializerFactory in standard)
        {
            var instance = (IYamlSerializerFactory)Activator.CreateInstance(serializerFactory)!;
            instance.Register(registry);
        }
        return registry;
    }
    /// <summary>
    /// Registers all available Serializers.
    /// May be removed in future.w
    /// Performance intensive Method.
    /// </summary>
    public static void Init()
    {
        lock (s_lock)
        {
            if (!s_isReady)
            {
                Instance.SerializerRegistry = new();
                // Get all loaded assemblies
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assemblies)
                {
                    var serializerFactories = assembly.GetTypes()
                        .Where(t => typeof(IYamlSerializerFactory).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    foreach (var serializerFactory in serializerFactories)
                    {
                        var instance = (IYamlSerializerFactory)Activator.CreateInstance(serializerFactory)!;
                        instance.Register(Instance);
                    }
                }

                s_isReady = true;
            }
        }
    }

    public void RegisterSerializer<T>(YamlSerializer<T> serializer)
    {
        var keyType = typeof(T);
        SerializerRegistry.DefinedSerializers[keyType] = serializer;
    }

    public void RegisterSerializer(Type serializer)
    {
        if (serializer.FullName is null)
        {
            throw new YamlException("FullName was null: " + serializer);
        }
        SerializerRegistry.TypeMap[serializer.FullName] = serializer;
    }

    public void RegisterTag(string tag, Type serializerType)
    {
        SerializerRegistry.TypeMap[tag] = serializerType;
        SerializerRegistry.AliasMap[serializerType] = tag;
    }

    public void RegisterGenericSerializer(Type target, Type serializerType)
    {
        SerializerRegistry.GenericSerializerBuffer.TryAdd(target, serializerType);
    }
}
internal class SerializerRegistry
{
    internal Dictionary<Type, Dictionary<Type, IYamlSerializerFactory>> SerializerFactory { get; } = new(new GenericEqualityComparer())
    {
    };
    internal Dictionary<Type, Type> GenericSerializerBuffer { get; } = new Dictionary<Type, Type>(new GenericEqualityComparer());
    internal Dictionary<string, Type> TypeMap { get; } = [];
    internal Dictionary<Type, string> AliasMap { get; } = [];
    internal Dictionary<Type, YamlSerializer> DefinedSerializers { get; } = new Dictionary<Type, YamlSerializer>()
    {
            // Primitive
            { typeof(short), Int16Serializer.Instance },
            { typeof(int), Int32Serializer.Instance },
            { typeof(long), Int64Serializer.Instance },
            { typeof(ushort), UInt16Serializer.Instance },
            { typeof(uint), UInt32Serializer.Instance },
            { typeof(ulong), UInt64Serializer.Instance },
            { typeof(float), Float32Serializer.Instance },
            { typeof(double), Float64Serializer.Instance },
            { typeof(bool), BooleanSerializer.Instance },
            { typeof(byte), ByteSerializer.Instance },
            { typeof(sbyte), SByteSerializer.Instance },
            { typeof(DateTime), DateTimeSerializer.Instance },
            { typeof(char), CharSerializer.Instance },
            // StandardClassLibrarySerializer
            { typeof(string), NullableStringSerializer.Instance },
            { typeof(decimal), DecimalSerializer.Instance },
            { typeof(TimeSpan), TimeSpanSerializer.Instance },
            { typeof(DateTimeOffset), DateTimeOffsetSerializer.Instance },
            { typeof(Guid), GuidSerializer.Instance },
            { typeof(Uri), UriSerializer.Instance },
    };
}
