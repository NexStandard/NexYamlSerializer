﻿using NexVYaml.Serialization;
using NexYamlSerializer.Serialization.Formatters;
using NexYamlSerializer.Serialization.PrimitiveSerializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NexVYaml;
/// <summary>
/// Default Implementation for a <see cref="IYamlFormatterResolver"/> Registry to retrieve the types from.
/// Will be used if no <see cref="IYamlFormatterResolver"/> is given in the <see cref="YamlSerializerOptions"/> when using <see cref="YamlSerializer"/>
/// </summary>
public class NexYamlSerializerRegistry : IYamlFormatterResolver
{
    public SerializerRegistry FormatterRegistry { get; set; } = new();
    public static NexYamlSerializerRegistry Instance { get; } = new NexYamlSerializerRegistry();

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
        return (YamlSerializer<T>?)Activator.CreateInstance(genericType);
    }

    public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType)
    {
        var tar = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
        var inter = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
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
            if(t is not null)
            {
                return t.Instantiate(origin);
            }
        }
        var genericFormatter = typeof(EmptyFormatter<>);
        var genericType = genericFormatter.MakeGenericType(origin);
        var emptyFormatter = (YamlSerializer?)Activator.CreateInstance(genericType);
        return emptyFormatter!;
    }
    /// <summary>
    /// Registers all available Serializers.
    /// May be removed in future.w
    /// Performance intensive Method.
    /// </summary>
    public static void Init()
    {
        Instance.FormatterRegistry = new();
        // Get all loaded assemblies
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Find types implementing IYamlFormatterHelper and invoke Register method
        foreach (var assembly in assemblies)
        {
            var formatterHelperTypes = assembly.GetTypes()
                .Where(t => typeof(IYamlFormatterHelper).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var formatterHelperType in formatterHelperTypes)
            {
                var instance = (IYamlFormatterHelper)Activator.CreateInstance(formatterHelperType)!;
                instance.Register(Instance);
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

    public void RegisterFormatter(Type formatter)
    {
        FormatterRegistry.TypeMap[formatter.FullName] = formatter;
    }

    public void RegisterTag(string tag, Type formatterGenericType)
    {
        FormatterRegistry.TypeMap[tag] = formatterGenericType;
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
        [typeof(ICollection<>)] = typeof(CollectionInterfaceFormatter<>),
        [typeof(IEnumerable<>)] = typeof(InterfaceEnumerableFormatter<>),
        [typeof(IList<>)] = typeof(InterfaceListFormatter<>),
        [typeof(IReadOnlyList<>)] = typeof(InterfaceReadOnlyListFormatter<>),
        [typeof(IReadOnlyCollection<>)] = typeof(InterfaceReadOnlyCollectionFormatter<>),
        [typeof(IDictionary<,>)] = typeof(DictionaryInterfaceFormatter<,>),
        [typeof(IReadOnlyDictionary<,>)] = typeof(DictionaryReadonlyInterfaceFormatter<,>),
        [typeof(KeyValuePair<,>)] = typeof(KeyValuePairFormatter<,>),
        [typeof(Action)] = typeof(DelegateFormatter<Action>),
        [typeof(Tuple<>)] = typeof(TupleFormatter<>),
        [typeof(Tuple<,>)] = typeof(TupleFormatter<,>),
        [typeof(Tuple<,,>)] = typeof(TupleFormatter<,,>),
        [typeof(Tuple<,,,>)] = typeof(TupleFormatter<,,,>),
        [typeof(Tuple<,,,,>)] = typeof(TupleFormatter<,,,,>),
        [typeof(Tuple<,,,,,>)] = typeof(TupleFormatter<,,,,,>),
        [typeof(Tuple<,,,,,,>)] = typeof(TupleFormatter<,,,,,,>),
        [typeof(Tuple<,,,,,,,>)] = typeof(TupleFormatter<,,,,,,,>)
    };
    internal Dictionary<string, Type> TypeMap { get; } = new()
    {
        ["!!del"] = typeof(Action)
    };
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
            { typeof(Action) , new DelegateFormatter<Action>() },
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