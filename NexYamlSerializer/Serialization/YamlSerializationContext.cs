#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NexVYaml.Emitter;
using NexVYaml.Internal;

namespace NexVYaml.Serialization
{
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
        static Type NullableFormatter = typeof(NullableFormatter<>);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize<T>(ref Utf8YamlEmitter emitter, T value)
        {
            var type = typeof(T);
            if (SecureMode)
            {
                if (type.IsGenericType)
                {
                    var protectedGeneric = Resolver.GetGenericFormatter<T>();
                    protectedGeneric.Serialize(ref emitter, value!, this);
                }
                else
                {
                    var protectedFormatter = Resolver.GetFormatter<T>();
                    protectedFormatter.Serialize(ref emitter, value!, this);

                }
                return;
            }
            IYamlFormatter formatter;
            if (IsNullable(type, out var underlyingType))
            {
                var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);

                ((IYamlFormatter<T>)Activator.CreateInstance(genericFilledFormatter, args: Resolver.GetFormatter(underlyingType))).Serialize(ref emitter, value, this);
            }
            else
            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                var valueType = value!.GetType();
                var formatt = this.Resolver.GetFormatter(value!.GetType(), typeof(T));
                if (valueType != type)
                    this.IsRedirected = true;
                // TODO : no idea how to make this without reflection
                // C# forgets the cast of T when invoking Deserialize,
                // this way we can call the deserialize method with the "real type"
                // that is in the object
                 var method = formatt.GetType().GetMethod("Serialize");
                 method.Invoke(formatt, new object[] { emitter, value, this });
            }
            else
            {
                Resolver.GetFormatter<T>().Serialize(ref emitter, value,this);
            }
        }
        public void SerializeArray<T>(ref Utf8YamlEmitter emitter, T[] value)
        {
            new ArrayFormatter<T>().Serialize(ref emitter, value, this);
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
}
