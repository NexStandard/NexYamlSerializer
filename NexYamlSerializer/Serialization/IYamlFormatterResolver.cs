#nullable enable
using NexYamlSerializer.Serialization.Formatters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace NexVYaml.Serialization
{
    public interface IYamlFormatterResolver
    {
        IYamlFormatter<T> GetFormatter<T>();
        public IYamlFormatter? GetFormatter(Type type);
        IYamlFormatter GetFormatter(Type type,Type origin);
        public void Register(IYamlFormatterHelper yamlFormatterHelper, Type target, Type interfaceType);
        IYamlFormatter<T>? GetGenericFormatter<T>();
        public Type GetAliasType(string alias);
        public void RegisterFormatter<T>(IYamlFormatter<T> formatter);
        public void RegisterTag(string tag, Type formatterGenericType);
        public void RegisterFormatter(Type formatter);
        public void RegisterGenericFormatter(Type target, Type formatterType);
        public static IYamlFormatterResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
    }

    public static class YamlFormatterResolverExtensions
    {
        static Type NullableFormatter = typeof(NullableFormatter<>);
        public static bool IsNullable(Type value, out Type underlyingType)
        {
            return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IYamlFormatter<T> GetFormatterWithVerify<T>(this IYamlFormatterResolver resolver)
        {
            IYamlFormatter<T>? formatter;
            try
            {
                var type = typeof(T);
                if (IsNullable(type, out var underlyingType))
                {
                        var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);

                        formatter = (IYamlFormatter<T>)Activator.CreateInstance(genericFilledFormatter, args:  resolver.GetFormatter(underlyingType));
                }
                else
                if (type.IsInterface || type.IsAbstract || type.IsGenericType)
                {
                    if(resolver is RedirectFormatter<T> redirector)
                    {
                        formatter = redirector;
                    }
                    else
                    {
                        formatter = new RedirectFormatter<T>();
                    }
                }
                else
                {
                    formatter = resolver.GetFormatter<T>();
                }
            }
            catch (TypeInitializationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException ?? ex).Throw();
                return default!; // not reachable
            }

            if (formatter != null)
            {
                return formatter;
            }
            Throw(typeof(T), resolver);
            return default!; // not reachable
        }
        
        static void Throw(Type t, IYamlFormatterResolver resolver)
        {
            throw new YamlSerializerException(t.FullName + $"{t} is not registered in resolver: {resolver.GetType()}");
        }
    }
}
