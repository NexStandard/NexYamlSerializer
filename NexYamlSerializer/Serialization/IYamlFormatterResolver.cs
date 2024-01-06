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
        IYamlFormatter<T>? GetFormatter<T>();
        public IYamlFormatter? GetFormatter(Type type);
        IYamlFormatter GetGenericFormatter(Type type,Type origin);
        IYamlFormatter<T>? GetGenericFormatter<T>();
        public Type GetAliasType(string alias);
        public IYamlFormatter FindFormatter<T>(Type target);
        public void RegisterFormatter<T>(IYamlFormatter<T> formatter);
        public void RegisterTag(string tag, Type formatterGenericType);
        public void RegisterFormatter(Type formatter);
        public void Register<T>(IYamlFormatter<T> formatter, Type interfaceType);
        public void Register(Type formatterType, Type interfaceType);
        public void RegisterGenericFormatter(Type target, Type formatterType);
        public static IYamlFormatterResolver Default { get; set; } = NexYamlSerializerRegistry.Instance;
    }

    public static class YamlFormatterResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IYamlFormatter<T> GetFormatterWithVerify<T>(this IYamlFormatterResolver resolver)
        {
            IYamlFormatter<T>? formatter;
            try
            {
                var type = typeof(T);
                if (type.IsInterface ||type.IsAbstract)
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
                    if(type.IsGenericType)
                    {
                        formatter = resolver.GetGenericFormatter<T>();
                        if (formatter is null)
                            return EmptyFormatter<T>.Empty();
                    }
                    else
                    {
                        formatter = resolver.GetFormatter<T>();
                    }

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
