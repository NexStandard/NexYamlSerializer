using System.Text;
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
        options ??= IYamlSerializerResolver.Default;
        StringWriter writer = new StringWriter(new StringBuilder());
        WriteContext<Node> node = new WriteContext<Node>(-2, true, style, new BlockMapping(), new YamlWriter(writer,options));

        node.WriteType(value, style);
        return writer.ToString();
    }

    public static void Write<T>(T value, TextWriter writer, DataStyle style = DataStyle.Any, IYamlSerializerResolver? options = null)
    {
        options ??= IYamlSerializerResolver.Default;
        writer ??= new StringWriter(new StringBuilder());
        WriteContext<Node> node = new WriteContext<Node>(-2, true, style, new BlockMapping(), new YamlWriter(writer, options));

        node.WriteType(value, style);
    }
}
