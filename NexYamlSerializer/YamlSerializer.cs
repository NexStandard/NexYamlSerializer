#nullable enable
using NexVYaml.Emitter;
using NexVYaml.Internal;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace NexVYaml;

public class YamlException : Exception
{
    public YamlException(string message) : base(message)
    {
    }

    public YamlException(Marker mark, string message) : base($"{message} at {mark}")
    {
    }
}
/// <summary>
/// Provides methods for serializing/deserializing objects to YAML format using the specified <see cref="YamlSerializerOptions"/>.
/// </summary>
public abstract class YamlSerializer
{
    /// <summary>
    /// Serializes the specified value to a <see cref="ReadOnlyMemory{T}"/> using YAML format.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="value">The value to serialize.</param>
    /// <param name="options">The serializer options (optional).</param>
    /// <returns>A read-only memory containing the serialized value.</returns>
    public static ReadOnlyMemory<byte> Serialize<T>(T value, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null)
    {
        options ??= IYamlFormatterResolver.Default;

        var emitter = new UTF8Stream();
        var stream = new YamlWriter(emitter, options);
        try
        {
            stream.Write(value,style);
            return emitter.Writer.WrittenMemory;
        }
        finally
        {
            emitter.Dispose();
        }
    }
    public static ReadOnlyMemory<byte> Serialize<T>(T? value, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null)
        where T : struct
    {
        if (value == null)
            return new ReadOnlyMemory<byte>();
        else
            return Serialize(value.Value, style, options);
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and writes it to the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="stream">The stream to which the YAML representation will be written.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    public static void Serialize<T>(T value, Stream stream, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null)
    {
        stream.Write(Serialize(value, style, options).Span);
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// using the provided <paramref name="emitter"/> and <paramref name="options"/> and disposes the emitter afterward.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="emitter">The Utf8YamlEmitter used for serializing the YAML content.</param>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    internal static void Serialize<T>(ref UTF8Stream emitter, T value, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null, IYamlWriter? stream = null)
    {
        try
        {
            options ??= IYamlFormatterResolver.Default;
            if(stream is null)
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
    /// Asynchronously serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and writes it to the given <paramref name="stream"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="stream">The stream to which the YAML representation will be written asynchronously.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async static Task SerializeAsync<T>(T value, Stream stream, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null)
    {
        await stream.WriteAsync(Serialize(value, style, options));
    }

    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> or any derived class/interface of type <typeparamref name="T"/> to YAML format 
    /// and returns the serialized content as a UTF-8 encoded string.
    /// </summary>
    /// <typeparam name="T">The type of the object to be serialized, or any type derived from <typeparamref name="T"/>.</typeparam>
    /// <param name="value">The object to be serialized.</param>
    /// <param name="options">Optional settings for customizing the YAML serialization process.</param>
    /// <returns>A UTF-8 encoded string representing the YAML serialization of the object.</returns>
    public static string SerializeToString<T>(T value, DataStyle style = DataStyle.Any, IYamlFormatterResolver? options = null)
    {
        var utf8Bytes = Serialize(value, style, options);
        return StringEncoding.Utf8.GetString(utf8Bytes.Span);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="memory"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="memory">The ReadOnlyMemory containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Deserialize<T>(ReadOnlyMemory<byte> memory, IYamlFormatterResolver? options = null)
    {
        var parser = YamlParser.FromSequence(new ReadOnlySequence<byte>(memory), options ?? IYamlFormatterResolver.Default);
        IYamlReader reader = new YamlReader(parser, options ?? IYamlFormatterResolver.Default);
        return Deserialize<T?>(reader, options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="yaml"/> string to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="yaml">The string containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Deserialize<T>(string yaml, IYamlFormatterResolver? options = null)
    {
        return Deserialize<T?>(StringEncoding.Utf8.GetBytes(yaml), options);
    }

    /// <summary>
    /// Deserializes the YAML content from the specified <paramref name="sequence"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="sequence">The ReadOnlySequence containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Deserialize<T>(in ReadOnlySequence<byte> sequence, IYamlFormatterResolver? options = null)
    {
        var parser = YamlParser.FromSequence(sequence, options ?? IYamlFormatterResolver.Default);
        IYamlReader reader = new YamlReader(parser, options ?? IYamlFormatterResolver.Default);
        return Deserialize<T?>(reader, options);
    }

    /// <summary>
    /// Deserializes the YAML content using the provided <paramref name="stream"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The YamlParser used for deserializing the YAML content.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>An object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static T? Deserialize<T>(IYamlReader stream, IYamlFormatterResolver? options = null)
    {
        try
        {
            options ??= IYamlFormatterResolver.Default;

            stream.SkipAfter(ParseEventType.DocumentStart);
            var value = default(T);
            stream.Read(ref value);
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
    public static async ValueTask<T?> DeserializeAsync<T>(Stream stream, IYamlFormatterResolver? options = null)
    {
        var byteSequenceBuilder = await StreamHelper.ReadAsSequenceAsync(stream);
        try
        {
            var sequence = byteSequenceBuilder.Build();
            return Deserialize<T?>(in sequence, options);
        }
        finally
        {
            ReusableByteSequenceBuilderPool.Return(byteSequenceBuilder);
        }
    }


    protected virtual DataStyle Style { get; } = DataStyle.Any;
    public abstract void Write(IYamlWriter stream, object value, DataStyle style);
    public abstract void Write(IYamlWriter stream, object value);
    public abstract void Read(IYamlReader parser, ref object? value);
}
public abstract class YamlSerializer<T> : YamlSerializer
{
    public override void Write(IYamlWriter stream, object value)
    {
        Write(stream, (T)value, Style);
    }
    public override void Write(IYamlWriter stream, object value, DataStyle style)
    {
        Write(stream, (T)value, style);
    }
    public override void Read(IYamlReader parser, ref object? value)
    {
        var val = (T)value!;
        Read(parser, ref val);
        value = val;
    }
    public abstract void Write(IYamlWriter stream, T value, DataStyle style);
    public abstract void Read(IYamlReader parser, [MaybeNull] ref T value);
}