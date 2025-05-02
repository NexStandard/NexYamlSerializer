using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Plugins;
/// <summary>
/// Provides the <see cref="IYamlWriter"/> with Syntax options to handle special cases during type resolution
/// Plugins resolve before any redirection, any plugin will be passed as long as the value is not consumed
/// </summary>
public interface IResolvePlugin
{
    /// <summary>
    /// Determines whether the given <paramref name="value"/> of type <typeparamref name="T"/> 
    /// requires special serialization handling before the standard serializers.
    /// </summary>
    /// <typeparam name="T">The type of the value to serialize.</typeparam>
    /// <param name="stream">The YAML writer to write the value to.</param>
    /// <param name="value">The value to be written.</param>
    /// <param name="provider">Specifies the desired YAML data style.</param>
    /// <returns>
    /// True if the value was successfully handled and written by this method; 
    /// false if the value was not processed and should be handled by the default serializers.
    /// </returns>
    bool Write<T>(IYamlWriter stream, T value, DataStyle provider);
    bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result);
    public static List<IResolvePlugin> plugins = new()
        {
            new NullPlugin(),
            new NullablePlugin(),
            new ArrayPlugin(),
            new DelegatePlugin(),
            new ReferencePlugin(),
        };
}