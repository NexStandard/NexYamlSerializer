using NexYamlSerializer.Serialization.Formatters;
using System;
using NexVYaml.Serialization;
using System.Linq;
using System.Buffers;
using System.IO;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;

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
    public IYamlFormatter GetGenericFormatter(Type type)
    {
        var genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type); ;
        if(genericFormatter is null)
            genericFormatter = typeof(EmptyFormatter<>);

        var genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
        return (IYamlFormatter)Activator.CreateInstance(genericType);
    }
    
    public void Register(IYamlFormatterHelper yamlFormatterHelper,Type target)
    {
        FormatterRegistry.Factories.Add(target, yamlFormatterHelper);
        
    }
    public void Register(IYamlFormatterHelper yamlFormatterHelper,Type target,Type interfaceType)
    {
        Type tar = target.IsGenericType ? target.GetGenericTypeDefinition() : target;
        Type inter = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        if(FormatterRegistry.Factories2.TryGetValue(inter, out var factory))
        {
            factory.TryAdd(tar, yamlFormatterHelper);
        }
        else
        {
            var dictionary = new Dictionary<Type, IYamlFormatterHelper>(new GenericEqualityComparer());
            FormatterRegistry.Factories2[inter] = dictionary;
            dictionary.Add(target, yamlFormatterHelper);
        }
    }
    public IYamlFormatter GetGenericFormatter(Type type, Type origin)
    {
        if (FormatterRegistry.Factories2.TryGetValue(origin, out var formatter))
        {
            try
            {
                formatter.TryGetValue(type, out var t);
                return t.Create(origin);
            }
            catch
            {
                return formatter[origin].Create(origin);
            }
        }
        
        var genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type);

        if (genericFormatter is null)
            genericFormatter = typeof(EmptyFormatter<>);
        
        var genericType = genericFormatter.MakeGenericType(origin.GenericTypeArguments.Take(genericFormatter.GetGenericArguments().Length).ToArray());
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


    // TODO: use for mapping generic interfaces??
    internal Type SynchronizeTypes(Type originalType,Type genericTarget)
    {
        return originalType;
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
        FormatterRegistry.GenericFormatterBuffer.Add(target, formatterType);
    }
    public void Register<T>(IYamlFormatter<T> formatter, Type interfaceType)
    {
        var keyType = typeof(T);
        if (!FormatterRegistry.FormatterBuffer.ContainsKey(interfaceType))
        {
            FormatterRegistry.FormatterBuffer.Add(interfaceType, new());
        }
        if (!FormatterRegistry.FormatterBuffer[interfaceType].ContainsKey(keyType))
        {
            FormatterRegistry.FormatterBuffer[interfaceType].Add(keyType, formatter);
        }
        else
        {
            FormatterRegistry.FormatterBuffer[interfaceType][keyType] = formatter;
        }
    }
    public void Register(Type formatterType, Type interfaceType)
    {
        if (!FormatterRegistry.FormatterBuffer.ContainsKey(interfaceType))
        {
            FormatterRegistry.FormatterBuffer.Add(interfaceType, new());
        }
        if (!FormatterRegistry.FormatterBuffer[interfaceType].ContainsKey(formatterType))
        {
            FormatterRegistry.FormatterBuffer[interfaceType].Add(formatterType, null);
        }
        else
        {
            FormatterRegistry.FormatterBuffer[interfaceType][formatterType] = null;
        }
    }
    public IYamlFormatter FindFormatter<T>(Type target)
    {
        if (FormatterRegistry.FormatterBuffer.ContainsKey(typeof(T)))
        {
            if (FormatterRegistry.FormatterBuffer[typeof(T)].TryGetValue(target, out var value))
            {
                return FormatterRegistry.FormatterBuffer[typeof(T)][target];
            }
        }
        if(FormatterRegistry.DefinedFormatters.TryGetValue(target, out var form))
        {
            return form;
        }
        return GetGenericFormatter(typeof(T), target);
    }
}
