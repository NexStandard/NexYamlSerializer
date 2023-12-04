#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace VYaml.Serialization
{
    public interface IYamlFormatterResolver
    {
        IYamlFormatter<T>? GetFormatter<T>();
    }

    public static class YamlFormatterResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IYamlFormatter<T> GetFormatterWithVerify<T>(this IYamlFormatterResolver resolver)
        {
            IYamlFormatter<T>? formatter;
            try
            {
                Type type = typeof(T);
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
                        formatter = NexYamlSerializerRegistry.Instance.GetGenericBufferedFormatter<T>();
                    }
                    else
                    {
                        formatter = GetRegistryFormatter<T>();
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
        private static IYamlFormatter<T> GetRegistryFormatter<T>()
        {
            return NexYamlSerializerRegistry.Instance.GetFormatter<T>();
        }
        public static IYamlFormatter<T> FindCompatibleFormatter<T>(this IYamlFormatterResolver resolver,T value,Type targetType,out bool IsRedirected)
        {
            Type rootType = typeof(T);
            IsRedirected = false;
            
            if (targetType == rootType)
                return GetFormatterWithVerify<T>(resolver);

            var formatter = NexYamlSerializerRegistry.Instance.GetFormatter(targetType);

            if(formatter == null)
                return null;

            IsRedirected = true;
            return (IYamlFormatter<T>)formatter;
            
        }
            static void Throw(Type t, IYamlFormatterResolver resolver)
        {
            throw new YamlSerializerException(t.FullName + $"{t} is not registered in resolver: {resolver.GetType()}");
        }
    }
}
