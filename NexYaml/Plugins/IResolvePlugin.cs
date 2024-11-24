using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.ResolvePlugin;
/// <summary>
/// Provides the <see cref="IYamlWriter"/> with Syntax options to handle special cases during type resolution
/// </summary>
public interface IResolvePlugin
{
    /// <summary>
    /// Investigates if the <paramref name="value"/> or the typeof <typeparamref name="T"/> needs special handling
    /// which can't be resolved through normal serializers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream">The stream to handle the case on</param>
    /// <param name="value">The value to handle</param>
    /// <returns>True if the value is consumed, false if the value couldn't be handled</returns>
    bool Write<T>(IYamlWriter stream, T value, DataStyle provider);
    bool Read<T>(IYamlReader stream, ref T value, ref ParseResult result);
}