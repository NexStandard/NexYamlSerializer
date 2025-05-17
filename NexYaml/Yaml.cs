using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serialization.Nodes;
using Stride.Core;
using System.Buffers;
using System.Text;

namespace NexYaml;

public class Yaml
{
    public static void Write<T>(T value, WriteDelegate writing, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        options ??= IYamlSerializerResolver.Default;
        List<IResolvePlugin> plugins = new()
        {
            new NullPlugin(),
            new NullablePlugin(),
            new ArrayPlugin(),
            new DelegatePlugin(),
            new ReferencePlugin(),
        };
        WriteContext<Node> node;

        node = new WriteContext<Node>(-2, true, style, new BlockMapping(), new DelegateWriter(options, plugins, writing));

        node.WriteType(value, style);
    }

    public static string Write<T>(T value, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        StringBuilder sb = new StringBuilder();
        Yaml.Write(value, (ReadOnlySpan<char> text) => { sb.Append(text); Console.Write(text.ToString()); }, style, options);
        return sb.ToString();
    }

    public static T? Read<T>(ReadOnlyMemory<char> memory, IYamlSerializerResolver? options = null)
    {
        return Read<T>(Encoding.UTF8.GetBytes(memory.Span.ToArray()), options);
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
        return Read<T?>(Encoding.UTF8.GetBytes(yaml), options);
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