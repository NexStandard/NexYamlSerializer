#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize<T>(ref Utf8YamlEmitter emitter, T value)
        {
            Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value, this);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SerializeCollection<T>(ref Utf8YamlEmitter emitter, T value)
            where T : ICollection<T>
        {
            Resolver.GetFormatterWithVerify<T>().Serialize(ref emitter, value, this);
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
