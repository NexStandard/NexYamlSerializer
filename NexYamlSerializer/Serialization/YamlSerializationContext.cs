#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexYamlSerializer.Emitter.Serializers;
using NexYamlSerializer.Serialization.Formatters;
using Stride.Core;

namespace NexVYaml.Serialization;

public readonly struct SequenceStyleScope
{
}

public readonly struct ScalarStyleScope
{
}

public class YamlSerializationContext : IDisposable
{
    public IYamlFormatterResolver Resolver { get; }
    public YamlEmitOptions EmitOptions { get; }
    /// <summary>
    /// Decides if the <see cref="RedirectFormatter{T}"/> had to redirect it as it's an interface or abstract class
    /// </summary>
    public bool IsRedirected { get; set; } = false;
    public bool IsFirst { get; set; } = true;
    public bool SecureMode { get; set; } = false;
    readonly byte[] primitiveValueBuffer;
    ArrayBufferWriter<byte>? arrayBufferWriter;

    public YamlSerializationContext(YamlSerializerOptions options)
    {
        primitiveValueBuffer = ArrayPool<byte>.Shared.Rent(64);
        Resolver = options.Resolver;
        EmitOptions = options.EmitOptions;
    }
    public static bool IsNullable(Type value, out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
    public void Serialize<T>(ref IYamlStream stream, T value, DataStyle style = DataStyle.Any)
    {
        var type = typeof(T);
        if (SecureMode)
        {
            if (type.IsGenericType)
            {
                var protectedGeneric = NewSerializerRegistry.Instance.GetGenericFormatter<T>();
                protectedGeneric ??= new EmptyFormatter<T>();
                protectedGeneric.Serialize(ref stream, value!, style);
            }
            else
            {
                var protectedFormatter = NewSerializerRegistry.Instance.GetFormatter<T>();
                protectedFormatter.Serialize(ref stream, value!, style);
            }
            return;
        }
        if (type.IsInterface || type.IsAbstract || type.IsGenericType || type.IsArray)
        {
                var valueType = value!.GetType();
                var formatt = NewSerializerRegistry.Instance.GetFormatter(value!.GetType(), typeof(T));
                if (valueType != type)
                    this.IsRedirected = true;

                // C# forgets the cast of T when invoking Deserialize,
                // this way we can call the deserialize method with the "real type"
                // that is in the object
                formatt.Serialize(ref stream, value!, style);
                // var method = formatt.GetType().GetMethod("Serialize");
                //method.Invoke(formatt, new object[] { emitter, value, this });
        }
        else
        {
            NewSerializerRegistry.Instance.GetFormatter<T>().Serialize(ref stream, value, style);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Serialize<T>(ref Utf8YamlEmitter emitter, T value, DataStyle style = DataStyle.Any)
    {
        var type = typeof(T);
        if (SecureMode)
        {
            if (type.IsGenericType)
            {
                var protectedGeneric = Resolver.GetGenericFormatter<T>();
                protectedGeneric ??= new EmptyFormatter<T>();
                protectedGeneric.Serialize(ref emitter, value!, this, style);
            }
            else
            {
                var protectedFormatter = Resolver.GetFormatter<T>();
                protectedFormatter.Serialize(ref emitter, value!, this, style);

        }
            return;
        }
        if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            var valueType = value!.GetType();
            var formatt = this.Resolver.GetFormatter(value!.GetType(), typeof(T));
            if (valueType != type)
                this.IsRedirected = true;

            // C# forgets the cast of T when invoking Deserialize,
            // this way we can call the deserialize method with the "real type"
            // that is in the object
            formatt.IndirectSerialize(ref emitter, value!, this, style);
            // var method = formatt.GetType().GetMethod("Serialize");
            //method.Invoke(formatt, new object[] { emitter, value, this });
        }
        else
        {
            Resolver.GetFormatter<T>().Serialize(ref emitter, value,this, style);
        }
    }
    public void SerializeArray<T>(ref Utf8YamlEmitter emitter, T[] value,DataStyle style = DataStyle.Any)
    {
        new ArrayFormatter<T>().Serialize(ref emitter, value, this, style);
    }
    public ArrayBufferWriter<byte> GetArrayBufferWriter()
    {
        return arrayBufferWriter ??= new ArrayBufferWriter<byte>(65536);
    }

    public void Reset()
    {
        arrayBufferWriter?.Clear();
    }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(primitiveValueBuffer);
    }

    public byte[] GetBuffer64() => primitiveValueBuffer;

    // readonly Stack<SequenceStyle> sequenceStyleStack = new();
    // readonly Stack<ScalarStyle> sequenceStyleStack = new();
}
