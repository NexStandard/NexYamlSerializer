using NexYamlSerializer.Serialization.Formatters;
using System;
using NexVYaml.Serialization;
using System.Linq;
using System.Collections.Generic;

namespace NexVYaml;
/// <summary>
/// Default Implementation for a <see cref="IYamlFormatterResolver"/> Registry to retrieve the types from.
/// Will be used if no <see cref="IYamlFormatterResolver"/> is given in the <see cref="YamlSerializerOptions"/> when using <see cref="YamlSerializer"/>
/// </summary>
public class NexYamlSerializerRegistry : IYamlFormatterResolver
{
    FormatterRegistry FormatterRegistry { get; set; } = new();
    public static NexYamlSerializerRegistry Instance { get; } = new NexYamlSerializerRegistry();
    
    internal NexYamlSerializerRegistry()
    {

    }
    public Type GetAliasType(string alias) => FormatterRegistry.TypeMap[alias];
    public IYamlFormatter<T> GetFormatter<T>()
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(typeof(T), out var formatter))
        {
            return (IYamlFormatter<T>)formatter;
        }
        return EmptyFormatter<T>.Empty();
    }

    public IYamlFormatter<T>? GetGenericFormatter<T>()
    {
        var type = typeof(T);

        var genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type);
        if (genericFormatter is null)
            return null;
        var genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
        return (IYamlFormatter<T>)Activator.CreateInstance(genericType);
    }

    public void Register(IYamlFormatterHelper yamlFormatterHelper,Type target,Type interfaceType)
    {
        Type tar = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
        Type inter = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        if(FormatterRegistry.FormatterFactories.TryGetValue(inter, out var factory))
        {
            factory.TryAdd(tar, yamlFormatterHelper);
        }
        else
        {
            var dictionary = new Dictionary<Type, IYamlFormatterHelper>(new GenericEqualityComparer());
            FormatterRegistry.FormatterFactories[inter] = dictionary;
            dictionary.Add(target, yamlFormatterHelper);
        }
    }
    public IYamlFormatter GetFormatter(Type type, Type origin)
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out var form))
        {
            return form;
        }
        if (FormatterRegistry.FormatterFactories.TryGetValue(origin, out var formatter))
        {
            formatter.TryGetValue(type, out var t);
            return t.Create(origin);
        }
        var genericFormatter = typeof(EmptyFormatter<>);
        
        var genericType = genericFormatter.MakeGenericType(origin);
        return (IYamlFormatter)Activator.CreateInstance(genericType);

    }
    /// <summary>
    /// Registers all available Serializers.
    /// May be removed in future.
    /// Performance intensive Method.
    /// </summary>
    public static void Init()
    {
        Instance.FormatterRegistry = new();
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

    public IYamlFormatter? GetFormatter(Type type)
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out var value))
        {
            return value;
        }
        return null;
    }
    public void RegisterFormatter<T>(IYamlFormatter<T> formatter)
    {
        var keyType = typeof(T);
        FormatterRegistry.DefinedFormatters[keyType] = formatter;
    }
    public void RegisterTag(string tag,Type formatterGenericType)
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
