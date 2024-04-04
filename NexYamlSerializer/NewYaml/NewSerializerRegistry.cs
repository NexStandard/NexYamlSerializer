using NexVYaml.Serialization;
using NexVYaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYamlSerializer.Serialization.Formatters;
using NexYamlSerializer.Serialization.PrimitiveSerializers;

namespace NexVYaml;
public class NewSerializerRegistry
{
    public SerializerRegistry FormatterRegistry { get; set; } = new();
    public static NewSerializerRegistry Instance { get; } = new NewSerializerRegistry();

    internal NewSerializerRegistry()
    {

    }
    public Type GetAliasType(string alias) => FormatterRegistry.TypeMap[alias];
    public YamlSerializer<T> GetFormatter<T>()
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(typeof(T), out var formatter))
        {
            return (YamlSerializer<T>)formatter;
        }
        return EmptyFormatter<T>.EmptyS();
    }

    public YamlSerializer<T>? GetGenericFormatter<T>()
    {
        var type = typeof(T);

        var genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type);
        if (genericFormatter is null)
            return null;
        var genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
        return (YamlSerializer<T>)Activator.CreateInstance(genericType);
    }

    public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType)
    {
        var tar = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
        Type inter = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        if (FormatterRegistry.FormatterFactories.TryGetValue(inter, out var factory))
        {
            factory.TryAdd(tar, yamlFormatterHelper);
        }
        else
        {
            var dictionary = new Dictionary<Type, IYamlFormatterHelper>(new GenericEqualityComparer());
            FormatterRegistry.FormatterFactories[inter] = dictionary;
            dictionary.TryAdd(target, yamlFormatterHelper);
        }
    }
    public YamlSerializer GetFormatter(Type type, Type origin)
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out var form))
        {
            return form;
        }
        if (FormatterRegistry.FormatterFactories.TryGetValue(origin, out var formatter))
        {
            formatter.TryGetValue(type, out var t);
            return t.Instantiate(origin);
        }
        var genericFormatter = typeof(EmptyFormatter<>);

        var genericType = genericFormatter.MakeGenericType(origin);
        return (YamlSerializer)Activator.CreateInstance(genericType);

    }
    /// <summary>
    /// Registers all available Serializers.
    /// May be removed in future.w
    /// Performance intensive Method.
    /// </summary>
    public static void Init()
    {
        NewSerializerRegistry.Instance.FormatterRegistry = new();
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Find types implementing IYamlForamtterHelper and invoke Register method
        foreach (var assembly in assemblies)
        {
            var formatterHelperTypes = assembly.GetTypes()
                .Where(t => typeof(IYamlFormatterHelper).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var formatterHelperType in formatterHelperTypes)
            {
                var instance = (IYamlFormatterHelper)Activator.CreateInstance(formatterHelperType);
                instance.Register(NexYamlSerializerRegistry.Instance);
            }
        }
    }

    public YamlSerializer? GetFormatter(Type type)
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out var value))
        {
            return value;
        }
        return null;
    }
    public void RegisterFormatter<T>(YamlSerializer<T> formatter)
    {
        var keyType = typeof(T);
        FormatterRegistry.DefinedFormatters[keyType] = formatter;
    }
    public void RegisterTag(string tag, Type formatterGenericType)
    {
        FormatterRegistry.TypeMap[tag] = formatterGenericType;
    }
    public void RegisterFormatter(Type formatter)
    {
        FormatterRegistry.TypeMap[formatter.FullName] = formatter;
    }
    public void RegisterGenericFormatter(Type target, Type formatterType)
    {
        FormatterRegistry.GenericFormatterBuffer.TryAdd(target, formatterType);
    }
}
public class SerializerRegistry
{
    internal Dictionary<Type, Dictionary<Type, IYamlFormatterHelper>> FormatterFactories { get; } = new(new GenericEqualityComparer());
    internal Dictionary<Type, Type> GenericFormatterBuffer { get; } = new Dictionary<Type, Type>(new GenericEqualityComparer())
    {
        [typeof(ICollection<>)] = typeof(InterfaceCollectionFormatter<>),
        [typeof(IEnumerable<>)] = typeof(InterfaceEnumerableFormatter<>),
        [typeof(IList<>)] = typeof(InterfaceListFormatter<>),
        [typeof(IReadOnlyList<>)] = typeof(InterfaceReadOnlyListFormatter<>),
        [typeof(IReadOnlyCollection<>)] = typeof(InterfaceReadOnlyCollectionFormatter<>),
        [typeof(Nullable<>)] = typeof(NullableFormatter<>),
        /*[typeof(IDictionary<,>)] = typeof(InterfaceDictionaryFormatter<,>),
        [typeof(IReadOnlyDictionary<,>)] = typeof(InterfaceReadOnlyDictionaryFormatter<,>),
        [typeof(Tuple<>)] = typeof(TupleFormatter<>),
        [typeof(Tuple<,>)] = typeof(TupleFormatter<,>),
        [typeof(Tuple<,,>)] = typeof(TupleFormatter<,,>),
        [typeof(Tuple<,,,>)] = typeof(TupleFormatter<,,,>),
        [typeof(Tuple<,,,,>)] = typeof(TupleFormatter<,,,,>),
        [typeof(Tuple<,,,,,>)] = typeof(TupleFormatter<,,,,,>),
        [typeof(Tuple<,,,,,,>)] = typeof(TupleFormatter<,,,,,,>),
        [typeof(Tuple<,,,,,,,>)] = typeof(TupleFormatter<,,,,,,,>)*/
    };
    internal Dictionary<string, Type> TypeMap { get; } = new();
    internal Dictionary<Type, YamlSerializer> DefinedFormatters { get; } = new Dictionary<Type, YamlSerializer>()
    {
            // Primitive
            { typeof(short), Int16Formatter.Instance },
            { typeof(int), Int32Formatter.Instance },
            { typeof(long), Int64Formatter.Instance },
            { typeof(ushort), UInt16Formatter.Instance },
            { typeof(uint), UInt32Formatter.Instance },
            { typeof(ulong), UInt64Formatter.Instance },
            { typeof(float), Float32Formatter.Instance },
            { typeof(double), Float64Formatter.Instance },
            { typeof(bool), BooleanFormatter.Instance },
            { typeof(byte), ByteFormatter.Instance },
            { typeof(sbyte), SByteFormatter.Instance },
            { typeof(DateTime), DateTimeFormatter.Instance },
            { typeof(char), CharFormatter.Instance },

            // StandardClassLibraryFormatter
            { typeof(string), NullableStringFormatter.Instance },
            { typeof(decimal), DecimalFormatter.Instance },
            { typeof(decimal?), new StaticNullableFormatter<decimal>((YamlSerializer<decimal>)DecimalFormatter.Instance) },
            { typeof(TimeSpan), TimeSpanFormatter.Instance },
            { typeof(TimeSpan?), new StaticNullableFormatter<TimeSpan>((YamlSerializer<TimeSpan>)TimeSpanFormatter.Instance) },
            { typeof(DateTimeOffset), DateTimeOffsetFormatter.Instance },
            { typeof(DateTimeOffset?), new StaticNullableFormatter<DateTimeOffset>((YamlSerializer<DateTimeOffset>)DateTimeOffsetFormatter.Instance) },
            { typeof(Guid), GuidFormatter.Instance },
            { typeof(Guid?), new StaticNullableFormatter<Guid>((YamlSerializer<Guid>)GuidFormatter.Instance) },
            { typeof(Uri), UriFormatter.Instance },
    };
}