using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;
using System.Buffers;

namespace NexYaml;

public class Yaml
{
    /// <summary>
    /// Serializes the specified value to a <see cref="ReadOnlyMemory{T}"/> using YAML format.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">The serializer options (optional).</param>
    /// <returns>A read-only memory containing the serialized value.</returns>
    public static string Write<T>(T value, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        options ??= IYamlSerializerResolver.Default;
        using var memoryStream = new MemoryStream();
        using var emitter = new UTF8BufferedStream(memoryStream);
        var stream = new YamlWriter(emitter, options);
        try
        {
            stream.Write(value, style);
            string result = "";
            emitter.Flush();
            memoryStream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        finally
        {
            emitter.Dispose();
        }
    }

    /// <summary>
    /// Serializes the specified value to a <see cref="ReadOnlyMemory{T}"/> using YAML format.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">The serializer options (optional).</param>
    /// <returns>A read-only memory containing the serialized value.</returns>
    public static void Write<T>(T value, Stream realStream, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        options ??= IYamlSerializerResolver.Default;

        var emitter = new UTF8BufferedStream(realStream);
        var stream = new YamlWriter(emitter, options);
        try
        {
            stream.Write(value, style);
        }
        finally
        {
            emitter.Dispose();
        }
    }
    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// using the provided <paramref name="emitter"/> and <paramref name="options"/> and disposes the emitter afterward.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="emitter">The Utf8YamlEmitter used for serializing the YAML content.</param>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    internal static void Write<T>(ref UTF8Stream emitter, T value, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null, IYamlWriter? stream = null)
    {
        try
        {
            options ??= IYamlSerializerResolver.Default;
            if (stream is null)
            {
                stream = new YamlWriter(emitter, options);
            }
            stream.Write(value, style);
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
    public static string WriteToString<T>(T value, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        var utf8Bytes = Write(value, style, options);
        return utf8Bytes.ToString();
    }

    public static T? Read<T>(ReadOnlyMemory<char> memory, IYamlSerializerResolver? options = null)
    {
        return Read<T>(StringEncoding.Utf8.GetBytes(memory.Span.ToArray()), options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="memory"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="memory">The ReadOnlyMemory containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Read<T>(ReadOnlyMemory<byte> memory, IYamlSerializerResolver? options = null)
    {
        var parser = YamlParser.FromSequence(new ReadOnlySequence<byte>(memory), options ?? IYamlSerializerResolver.Default);
        IYamlReader reader = new YamlReader(parser, options ?? IYamlSerializerResolver.Default);
        return Read<T?>(reader, options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="yaml"/> string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="yaml">The string containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Read<T>(string yaml, IYamlSerializerResolver? options = null)
    {
        return Read<T?>(StringEncoding.Utf8.GetBytes(yaml), options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="sequence"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="sequence">The ReadOnlySequence containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Read<T>(in ReadOnlySequence<byte> sequence, IYamlSerializerResolver? options = null)
    {
        var parser = YamlParser.FromSequence(sequence, options ?? IYamlSerializerResolver.Default);
        IYamlReader reader = new YamlReader(parser, options ?? IYamlSerializerResolver.Default);
        return Read<T?>(reader, options);
    }

    /// <summary>
    /// Deserializes the YAML content using the provided <paramref name="stream"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The YamlParser used for deserializing the YAML content.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Read<T>(IYamlReader stream, IYamlSerializerResolver? options = null)
    {
        try
        {
            options ??= IYamlSerializerResolver.Default;

            stream.SkipAfter(ParseEventType.DocumentStart);
            var value = default(T);
            stream.Read(ref value);
            stream.ResolveReferences();
            return value;
        }
        finally
        {
            stream.Dispose();
        }
    }

    /// <summary>
    /// Asynchronously deserializes the YAML content from the specified <paramref name="stream"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The Stream containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with the result being an object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static async ValueTask<T?> ReadAsync<T>(Stream stream, IYamlSerializerResolver? options = null)
    {
        var byteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream);
        try
        {
            var sequence = byteSequenceBuilder.Build();
            return Read<T?>(in sequence, options);
        }
        finally
        {
            ReusableByteSequenceBuilderPool.Return(byteSequenceBuilder);
        }
    }
}