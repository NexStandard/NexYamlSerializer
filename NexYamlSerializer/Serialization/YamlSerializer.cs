#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Parser;
using NexVYaml.Serialization;

namespace NexVYaml
{
    public class YamlSerializerException : Exception
    {
        public YamlSerializerException(string message) : base(message)
        {
        }

        public YamlSerializerException(Marker mark, string message) : base($"{message} at {mark}")
        {
        }
    }
    /// <summary>
    /// Provides methods for serializing/deserializing objects to YAML format using the specified <see cref="YamlSerializerOptions"/>.
    /// </summary>
    public static class YamlSerializer
    {
        /// <summary>
        /// Gets or sets the default serialization <see cref="YamlSerializerOptions"/> used by the YamlSerializer if no <see cref="YamlSerializerOptions"/> is given.
        /// </summary>
        public static YamlSerializerOptions DefaultOptions
        {
            get => defaultOptions ??= YamlSerializerOptions.Standard;
            set => defaultOptions = value;
        }

        static YamlSerializerOptions? defaultOptions;

        /// <summary>
        /// Serializes the specified value to a <see cref="ReadOnlyMemory{T}"/> using YAML format.
        /// </summary>
        /// <typeparam name="T">The type of the value to serialize.</typeparam>
        /// <param name="value">The value to serialize.</param>
        /// <param name="options">The serializer options (optional).</param>
        /// <returns>A read-only memory containing the serialized value.</returns>
        public static ReadOnlyMemory<byte> Serialize<T>(T value, YamlSerializerOptions? options = null)
        {
            options ??= DefaultOptions;

            var contextLocal = new YamlSerializationContext(options)
            {
                SecureMode = options.SecureMode,
            };

            var writer = contextLocal.GetArrayBufferWriter();

            var emitter = new Utf8YamlEmitter(writer);

            try
            {
                new RedirectFormatter<T>().Serialize(ref emitter, value, contextLocal);

                return writer.WrittenMemory;
            }
            finally
            {
                emitter.Dispose();
            }
        }


        public static void Serialize<T>(T value, Stream stream,YamlSerializerOptions? options = null)
        {
            stream.Write(Serialize(value, options).Span);
        }

        public async static Task SerializeAsync<T>(T value, Stream stream, YamlSerializerOptions? options = null)
        {
            await stream.WriteAsync(Serialize(value, options));
        }

        public static void Serialize<T>(IBufferWriter<byte> writer, T value, YamlSerializerOptions? options = null)
        {
            var emitter = new Utf8YamlEmitter(writer);
            Serialize(ref emitter, value, options);
        }

        public static void Serialize<T>(ref Utf8YamlEmitter emitter, T value, YamlSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                var contextLocal = new YamlSerializationContext(options);

                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                formatter.Serialize(ref emitter, value, contextLocal);
            }
            finally
            {
                emitter.Dispose();
            }
        }

        public static string SerializeToString<T>(T value, YamlSerializerOptions? options = null)
        {
            var utf8Bytes = Serialize(value, options);
            return StringEncoding.Utf8.GetString(utf8Bytes.Span);
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> memory, YamlSerializerOptions? options = null)
        {
            var parser = YamlParser.FromSequence(new ReadOnlySequence<byte>(memory));
            return Deserialize<T>(ref parser, options);
        }
        public static T Deserialize<T>(string yaml, YamlSerializerOptions? options = null)
        {
            return Deserialize<T>(Encoding.UTF8.GetBytes(yaml), options);
        }
        public static T Deserialize<T>(in ReadOnlySequence<byte> sequence, YamlSerializerOptions? options = null)
        {
            var parser = YamlParser.FromSequence(sequence);
            return Deserialize<T>(ref parser, options);
        }

        public static async ValueTask<T> DeserializeAsync<T>(Stream stream, YamlSerializerOptions? options = null)
        {
            var byteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream);
            try
            {
                var sequence = byteSequenceBuilder.Build();
                return Deserialize<T>(in sequence, options);
            }
            finally
            {
                ReusableByteSequenceBuilderPool.Return(byteSequenceBuilder);
            }
        }

        public static T Deserialize<T>(ref YamlParser parser, YamlSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                var contextLocal = new YamlDeserializationContext(options);

                parser.SkipAfter(ParseEventType.DocumentStart);

                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                return contextLocal.DeserializeWithAlias(formatter, ref parser);
            }
            finally
            {
                parser.Dispose();
            }
        }

        public static async ValueTask<IEnumerable<T>> DeserializeMultipleDocumentsAsync<T>(Stream stream, YamlSerializerOptions? options = null)
        {
            var byteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream);
            try
            {
                var sequence = byteSequenceBuilder.Build();
                return DeserializeMultipleDocuments<T>(in sequence, options);
            }
            finally
            {
                ReusableByteSequenceBuilderPool.Return(byteSequenceBuilder);
            }
        }

        public static IEnumerable<T> DeserializeMultipleDocuments<T>(ReadOnlyMemory<byte> memory, YamlSerializerOptions? options = null)
        {
            var parser = YamlParser.FromSequence(new ReadOnlySequence<byte>(memory));
            return DeserializeMultipleDocuments<T>(ref parser, options);
        }

        public static IEnumerable<T> DeserializeMultipleDocuments<T>(in ReadOnlySequence<byte> sequence, YamlSerializerOptions? options = null)
        {
            var parser = YamlParser.FromSequence(sequence);
            return DeserializeMultipleDocuments<T>(ref parser, options);
        }

        public static IEnumerable<T> DeserializeMultipleDocuments<T>(ref YamlParser parser, YamlSerializerOptions? options = null)
        {
            try
            {
                options ??= DefaultOptions;
                var contextLocal = new YamlDeserializationContext(options);
                var formatter = options.Resolver.GetFormatterWithVerify<T>();
                var documents = new List<T>();

                while (true)
                {
                    parser.SkipAfter(ParseEventType.DocumentStart);
                    if (parser.End)
                    {
                        break;
                    }

                    contextLocal.Reset();
                    var document = contextLocal.DeserializeWithAlias(formatter, ref parser);
                    documents.Add(document);
                }
                return documents;
            }
            finally
            {
                parser.Dispose();
            }
        }
    }
}
