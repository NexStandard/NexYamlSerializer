using NexYamlSerializer.Serialization.Formatters;
using System;
using NexVYaml.Serialization;
using System.Linq;

namespace NexVYaml;

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
    internal IYamlFormatter<T>? GetGenericFormatter<T>()
    {
        Type type = typeof(T);
        Type genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type); ;
        if (genericFormatter is null)
            return null;
        Type genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
        return (IYamlFormatter<T>)Activator.CreateInstance(genericType);
    }
    public IYamlFormatter GetGenericFormatter(Type type)
    {
        Type genericFormatter = FormatterRegistry.GenericFormatterBuffer.FindAssignableType(type); ;
        if(genericFormatter is null)
            genericFormatter = typeof(EmptyFormatter<>);
        Type genericType = genericFormatter.MakeGenericType(type.GenericTypeArguments);
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
                var instance = Activator.CreateInstance(formatterHelperType);

                // Assuming Register method has no parameters
                var registerMethod = formatterHelperType.GetMethod("Register");
                if (registerMethod != null)
                {
                    registerMethod.Invoke(instance, null);
                }
            }
        }
    }


    internal Type SynchronizeTypes(Type originalType,Type genericTarget)
    {
        return originalType;
    }
    public IYamlFormatter? GetFormatter(Type type)
    {
        if (FormatterRegistry.DefinedFormatters.TryGetValue(type, out IYamlFormatter value))
        {
            return value;
        }
        return GetGenericFormatter(type);
    }
    public void RegisterFormatter<T>(IYamlFormatter<T> formatter)
    {
        Type keyType = typeof(T);
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
        Type keyType = typeof(T);
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
            if (FormatterRegistry.FormatterBuffer[typeof(T)].TryGetValue(target, out IYamlFormatter value))
            {
                return FormatterRegistry.FormatterBuffer[typeof(T)][target];
            }
        }
        return GetGenericFormatter(target);
    }
}
