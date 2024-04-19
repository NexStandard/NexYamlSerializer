#nullable enable
using NexVYaml.Emitter;
using NexYamlSerializer.Serialization.Formatters;
using Stride.Core;
using System;
using System.Buffers;

namespace NexVYaml.Serialization;

public class YamlSerializationContext
{
    public IYamlFormatterResolver Resolver { get; }
    public YamlEmitOptions EmitOptions { get; }
    /// <summary>
    /// Decides if the <see cref="RedirectFormatter{T}"/> had to redirect it as it's an interface or abstract class
    /// </summary>
    public bool IsRedirected { get; set; } = false;
    public bool IsFirst { get; set; } = true;
    public bool SecureMode { get; set; } = false;
    ArrayBufferWriter<byte>? arrayBufferWriter;

    public YamlSerializationContext(YamlSerializerOptions options)
    {
        Resolver = options.Resolver;
        EmitOptions = options.EmitOptions;
    }

    public void Serialize<T>(ISerializationWriter stream, T value, DataStyle style = DataStyle.Any)
    {
        var type = typeof(T);
        if (SecureMode)
        {
            if (type.IsGenericType)
            {
                var protectedGeneric = Resolver.GetGenericFormatter<T>();
                protectedGeneric ??= new EmptyFormatter<T>();
                protectedGeneric.Serialize(ref stream, value!, style);
            }
            else
            {
                var protectedFormatter = Resolver.GetFormatter<T>();
                protectedFormatter.Serialize(ref stream, value!, style);
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
                formatt.Serialize(ref stream, value!);
            }
            else
            {
                formatt.Serialize(ref stream, value!, style);
            }
            // var method = formatt.GetType().GetMethod("Serialize");
            //method.Invoke(formatt, new object[] { emitter, value, this });
        }
        else
        {
            if(style is DataStyle.Any)
            {
                Resolver.GetFormatter<T>().Serialize(ref stream, value!);
            }
            else
            {
                Resolver.GetFormatter<T>().Serialize(ref stream, value, style);
            }
        }
    }
    public ArrayBufferWriter<byte> GetArrayBufferWriter()
    {
        return arrayBufferWriter ??= new ArrayBufferWriter<byte>(512);
    }
}
