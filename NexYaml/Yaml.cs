using System.Buffers;
using System.Text;
using NexYaml.Parser;
using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serialization.Nodes;
using Stride.Core;

namespace NexYaml;
/// <summary>
/// Provides static methods for serializing objects to YAML and deserializing YAML content.
/// </summary>
public static class Yaml
{
    /// <summary>
    /// Serializes the specified value as YAML and writes the output to the provided <see cref="WriteDelegate"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized.</typeparam>
    /// <param name="value">The value to serialize as YAML.</param>
    /// <param name="writing">A delegate to handle the output of the serialized YAML text.</param>
    /// <param name="style">The <see cref="DataStyle"/> determining the formatting of the YAML output. The default is <see cref="DataStyle.Any"/>.</param>
    /// <param name="options">
    /// The options used by the YAML serializer resolver to control serialization features.
    /// If not specified, the default resolver <see cref="IYamlSerializerResolver.Default"/> will be used.
    /// </param>
    public static void Write<T>(T value, WriteDelegate writing, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        options ??= IYamlSerializerResolver.Default;
        List<IResolvePlugin> plugins = new()
        {
            new ArrayPlugin(),
            new ReferencePlugin(),
        };
        WriteContext<Node> node = new WriteContext<Node>(-2, true, style, new BlockMapping(), new DelegateWriter(options, plugins, writing));

        node.WriteType(value, style);
    }

    /// <summary>
    /// Serializes the specified value to a YAML formatted string.
    /// </summary>
    /// <typeparam name="T">The type of the value to be serialized.</typeparam>
    /// <param name="value">The value to serialize as YAML.</param>
    /// <param name="style">The <see cref="DataStyle"/> determining the formatting of the YAML output. The default is <see cref="DataStyle.Any"/>.</param>
    /// <param name="options">
    /// The options used by the YAML serializer resolver to control serialization features.
    /// If not specified, the default resolver <see cref="IYamlSerializerResolver.Default"/> will be used.
    /// </param>
    /// <remarks>
    /// may be obsoleted/renamed at some point
    /// </remarks>
    /// <returns>A string containing the YAML formatted representation of the value.</returns>
    public static string Write<T>(T value, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        StringBuilder sb = new StringBuilder();
        Yaml.Write(value, (ReadOnlySpan<char> text) => { sb.Append(text); Console.Write(text.ToString()); }, style, options);
        return sb.ToString();
    }

    /// <summary>
    /// Asynchronously deserializes YAML content from a string into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to be deserialized from the <paramref name="yamlString"/>.</typeparam>
    /// <param name="yamlString">A string containing YAML content.</param>
    /// <param name="options">
    /// The options used by the YAML serializer resolver to control deserialization features.
    /// If not specified, the default resolver <see cref="IYamlSerializerResolver.Default"/> will be used.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask{T}"/> with the resulting object of type <typeparamref name="T"/> upon completion.
    /// </returns>
    public static async ValueTask<T?> Read<T>(string yamlString, IYamlSerializerResolver? options = null)
    {
        var sequence = new ReadOnlySequence<char>(yamlString.ToArray());
        using var parser = YamlParser.FromSequence(sequence, options ?? IYamlSerializerResolver.Default);
        using var reader = new YamlReader(parser, options ?? IYamlSerializerResolver.Default);

        parser.SkipAfter(ParseEventType.DocumentStart);
        return await reader.Read<T>(new());
    }
}
