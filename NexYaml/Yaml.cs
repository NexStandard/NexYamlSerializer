using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serialization.Nodes;
using Stride.Core;
using System.Buffers;
using System.IO;
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

    /// <summary>
    /// Asynchronously deserializes the YAML content from the specified <paramref name="stream"/> to an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized.</typeparam>
    /// <param name="stream">The Stream containing the YAML content to be deserialized.</param>
    /// <param name="options">Optional settings for customizing the YAML deserialization process.</param>
    /// <returns>A ValueTask representing the asynchronous operation, with the result being an object of type <typeparamref name="T"/> representing the deserialized YAML content.</returns>
    public static async ValueTask<T?> ReadAsync<T>(string s, IYamlSerializerResolver? options = null)
    {
        var sequence = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes(s));
        var parser = YamlParser.FromSequence(sequence, options ?? IYamlSerializerResolver.Default);
        YamlReader reader = new YamlReader(parser, options ?? IYamlSerializerResolver.Default);
        options ??= IYamlSerializerResolver.Default;

        parser.SkipAfter(ParseEventType.DocumentStart);
        var value = default(T);
        var x = await reader.Read<T>(new());
        return x;
    }
}