using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serializers;
using System.Reflection;
using System.Runtime.CompilerServices;

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
    public string GetTypeAlias(Type type)
    {
        return SerializerRegistry.AliasMap[type];
    }
    public YamlSerializer<T> GetSerializer<T>()
    {
        if (SerializerRegistry.DefinedSerializers.TryGetValue(typeof(T), out var serializer))
        {
            return (YamlSerializer<T>)serializer;
        }
        return EmptySerializer<T>.EmptyS();
    }

    public YamlSerializer<T>? GetGenericSerializer<T>()
    {
        var type = typeof(T);

        var genericSerializer = SerializerRegistry.GenericSerializerBuffer.FindAssignableType(type);
        if (genericSerializer is null)
            return null;
        var genericType = genericSerializer.MakeGenericType(type.GenericTypeArguments);
        return (YamlSerializer<T>?)Activator.CreateInstance(genericType);
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
        if(SerializerRegistry.DefinedSerializers.TryGetValue(origin, out var serializer2))
        {
            return serializer2;
        }
        // search if there is a 
        if (SerializerRegistry.SerializerFactory.TryGetValue(origin, out var serializer))
        {
            serializer.TryGetValue(type, out var t);
            if (t is not null)
            {
                return t.Instantiate(origin);
            }
        }
        var genericSerializer = typeof(EmptySerializer<>);
        var genericType = genericSerializer.MakeGenericType(origin);
        var emptySerializer = (YamlSerializer?)Activator.CreateInstance(genericType);
        return emptySerializer!;
    }
    public static bool IsReady = false;
#if NET9_0_OR_GREATER
    static Lock s = new();
#elif NET8_0
    static object s = new object();
#endif
    public static IYamlSerializerResolver Create(Assembly assembly)
    {
        var registry = new NexYamlSerializerRegistry();
        registry.SerializerRegistry = new();
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
        lock (s)
        {
            if (!IsReady)
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
               
                IsReady = true;
            }
        }

    }

    public YamlSerializer? GetSerializer(Type type)
    {
        if (SerializerRegistry.DefinedSerializers.TryGetValue(type, out var value))
        {
            return value;
        }
        return null;
    }

    public void RegisterSerializer<T>(YamlSerializer<T> serializer)
    {
        var keyType = typeof(T);
        SerializerRegistry.DefinedSerializers[keyType] = serializer;
    }

    public void RegisterSerializer(Type serializer)
    {
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
    internal Dictionary<Type, Type> GenericSerializerBuffer { get; } = new Dictionary<Type, Type>(new GenericEqualityComparer())
    {
        [typeof(KeyValuePair<,>)] = typeof(KeyValuePairSerializer<,>),
        [typeof(Tuple<>)] = typeof(TupleSerializer<>),
        [typeof(Tuple<,>)] = typeof(TupleSerializer<,>),
        [typeof(Tuple<,,>)] = typeof(TupleSerializer<,,>),
        [typeof(Tuple<,,,>)] = typeof(TupleSerializer<,,,>),
        [typeof(Tuple<,,,,>)] = typeof(TupleSerializer<,,,,>),
        [typeof(Tuple<,,,,,>)] = typeof(TupleSerializer<,,,,,>),
        [typeof(Tuple<,,,,,,>)] = typeof(TupleSerializer<,,,,,,>),
        [typeof(Tuple<,,,,,,,>)] = typeof(TupleSerializer<,,,,,,,>)
    };
    internal Dictionary<string, Type> TypeMap { get; } = new()
    {
    };
    internal Dictionary<Type, string> AliasMap { get; } = new()
    {
    };
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
            { typeof(decimal?), DecimalSerializer.Instance },
            { typeof(TimeSpan), TimeSpanSerializer.Instance },
            { typeof(TimeSpan?), TimeSpanSerializer.Instance },
            { typeof(DateTimeOffset), DateTimeOffsetSerializer.Instance },
            { typeof(DateTimeOffset?), DateTimeOffsetSerializer.Instance },
            { typeof(Guid), GuidSerializer.Instance },
            { typeof(Guid?), GuidSerializer.Instance },
            { typeof(Uri), UriSerializer.Instance },
    };
}