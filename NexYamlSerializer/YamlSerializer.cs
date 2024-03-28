#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Parser;
using NexVYaml.Serialization;

namespace NexVYaml;

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
            contextLocal.Serialize(ref emitter, value);

            return writer.WrittenMemory;
        }
        finally
        {
            emitter.Dispose();
        }
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and writes it to the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="stream">The stream to which the YAML representation will be written.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    public static void Serialize<T>(T value, Stream stream, YamlSerializerOptions? options = null)
    {
        stream.Write(Serialize(value, options).Span);
    }

    /// <summary>
    /// Asynchronously serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and writes it to the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="stream">The stream to which the YAML representation will be written asynchronously.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async static Task SerializeAsync<T>(T value, Stream stream, YamlSerializerOptions? options = null)
    {
        await stream.WriteAsync(Serialize(value, options));
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and writes it to the provided <paramref name="writer"/> using an <see cref="IBufferWriter{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="writer">The buffer writer used to write the YAML content.</param>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    public static void Serialize<T>(IBufferWriter<byte> writer, T value, YamlSerializerOptions? options = null)
    {
        var emitter = new Utf8YamlEmitter(writer);
        Serialize(ref emitter, value, options);
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// using the provided <paramref name="emitter"/> and <paramref name="options"/> and disposes the emitter afterward.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="emitter">The Utf8YamlEmitter used for serializing the YAML content.</param>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    public static void Serialize<T>(ref Utf8YamlEmitter emitter, T value, YamlSerializerOptions? options = null)
    {
        try
        {
            options ??= DefaultOptions;
            var contextLocal = new YamlSerializationContext(options);

            contextLocal.Serialize(ref emitter, value);
        }
        finally
        {
            emitter.Dispose();
        }
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and returns the serialized content as a UTF-8 encoded string.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    /// <returns>A UTF-8 encoded string representing the YAML serialization of the object.</returns>
    public static string SerializeToString<T>(T value, YamlSerializerOptions? options = null)
    {
        var utf8Bytes = Serialize(value, options);
        return StringEncoding.Utf8.GetString(utf8Bytes.Span);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="memory"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="memory">The ReadOnlyMemory containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T Deserialize<T>(ReadOnlyMemory<byte> memory, YamlSerializerOptions? options = null)
    {
        var parser = YamlParser.FromSequence(new ReadOnlySequence<byte>(memory));
        return Deserialize<T>(ref parser, options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="yaml"/> string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="yaml">The string containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T Deserialize<T>(string yaml, YamlSerializerOptions? options = null)
    {
        return Deserialize<T>(Encoding.UTF8.GetBytes(yaml), options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="sequence"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="sequence">The ReadOnlySequence containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T Deserialize<T>(in ReadOnlySequence<byte> sequence, YamlSerializerOptions? options = null)
    {
        var parser = YamlParser.FromSequence(sequence);
        return Deserialize<T>(ref parser, options);
    }

    /// <summary>
    /// Asynchronously deserializes the YAML content from the specified <paramref name="stream"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The Stream containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with the result being an object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
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

    /// <summary>
    /// Deserializes the YAML content using the provided <paramref name="parser"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="parser">The YamlParser used for deserializing the YAML content.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Deserialize<T>(ref YamlParser parser, YamlSerializerOptions? options = null)
    {
        try
        {
            options ??= DefaultOptions;
            var contextLocal = new YamlDeserializationContext(options);

            parser.SkipAfter(ParseEventType.DocumentStart);

            return contextLocal.DeserializeWithAlias<T>( ref parser);
        }
        finally
        {
            parser.Dispose();
        }
    }
}
