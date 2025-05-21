using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Plugins;

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
    bool Write<T,X>(WriteContext<X> context,T value, DataStyle style)
        where X : Node;
    bool Read<T>(IYamlReader stream, out ValueTask<T?> value, ParseContext result);
}