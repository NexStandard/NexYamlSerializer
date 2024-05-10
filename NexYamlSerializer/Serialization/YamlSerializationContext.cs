#nullable enable
using NexVYaml.Emitter;
using NexYamlSerializer.Serialization.Formatters;
using Stride.Core;
using System;
using System.Buffers;

namespace NexVYaml.Serialization;

/// <summary>
/// Enforces <see cref="DataStyle.Compact"/> 
/// 
/// </summary>
class StyleEnforcer
{
    int count;
    public void Begin(ref DataStyle style)
    {
        if(style is DataStyle.Compact || count > 0)
        {
            style = DataStyle.Compact;
            count++;
        }
    }
    public void End()
    {
        if(count > 0)
        {
            count--;
        }
    }
}
public class YamlSerializationContext(YamlSerializerOptions options)
{
    public IYamlFormatterResolver Resolver { get; } = options.Resolver;
    public YamlEmitOptions EmitOptions { get; } = options.EmitOptions;
    /// <summary>
    /// Decides if the <see cref="RedirectFormatter{T}"/> had to redirect it as it's an interface or abstract class
    /// </summary>
    public bool IsRedirected { get; set; } = false;
    public bool IsFirst { get; set; } = true;
    public bool SecureMode { get; set; } = false;
    ArrayBufferWriter<byte>? arrayBufferWriter;

    public void Serialize<T>(SerializationWriter stream, T value, DataStyle style = DataStyle.Any)
    {

        var type = typeof(T);
        if (SecureMode)
        {
            if (type.IsGenericType)
            {
                var protectedGeneric = Resolver.GetGenericFormatter<T>();
                protectedGeneric ??= new EmptyFormatter<T>();
                protectedGeneric.Serialize(stream, value!, style);
            }
            else
            {
                var protectedFormatter = Resolver.GetFormatter<T>();
                protectedFormatter.Serialize(stream, value!, style);
            }
            return;
        }
        if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
            var valueType = value!.GetType();
            var formatt = Resolver.GetFormatter(value!.GetType(), typeof(T));
            if (valueType != type)
                IsRedirected = true;

            // C# forgets the cast of T when invoking Serialize,
            // this way we can call the serialize method with the "real type"
            // that is in the object
            if(style is DataStyle.Any)
            {
                formatt.Serialize(stream, value!);
            }
            else
            {
                formatt.Serialize(stream, value!, style);
            }
        }
        else
        {
            if(style is DataStyle.Any)
            {
                Resolver.GetFormatter<T>().Serialize(stream, value!);
            }
            else
            {
                Resolver.GetFormatter<T>().Serialize(stream, value!, style);
            }
        }
    }
    public ArrayBufferWriter<byte> GetArrayBufferWriter()
    {
        return arrayBufferWriter ??= new ArrayBufferWriter<byte>(512);
    }
}
