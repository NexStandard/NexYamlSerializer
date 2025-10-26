using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.XParser;
using Stride.Core;

namespace NexYaml;
/// <summary>
/// Provides methods for serializing/deserializing objects to YAML format.
/// </summary>
public abstract class YamlSerializer
{
    /// <summary>
    /// Gets the prefered <see cref="DataStyle"/>
    /// </summary>
    protected virtual DataStyle Style { get; } = DataStyle.Any;

    /// <summary>
    /// Serializes the specified object into YAML.
    /// </summary>
    /// <typeparam name="X">The <see cref="Node"/> type.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public abstract void Write<X>(WriteContext<X> context, object value, DataStyle style) where X : Node;

    /// <summary>
    /// Deserializes YAML content into an object.
    /// </summary>
    /// <param name="stream">The <see cref="IYamlReader"/>.</param>
    /// <param name="parseResult">The <see cref="ParseContext"/>.</param>
    /// <returns>A task yielding the deserialized object.</returns>
    public abstract ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext parseResult);

    public abstract ValueTask<object?> ReadUnknown(Scope scope, ParseContext context);
}
/// <summary>
/// Provides methods for serializing and deserializing objects of type <typeparamref name="T"/> to/from YAML.
/// </summary>
/// <typeparam name="T">The type handled by the serializer.</typeparam>
public abstract class YamlSerializer<T> : YamlSerializer
{
    public override void Write<X>(WriteContext<X> context, object value, DataStyle style)
    {
        Write(context, (T)value, style);
    }
    public override async ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext parseResult)
    {
        return await Read(stream, parseResult);
    }
    public override async ValueTask<object?> ReadUnknown(Scope scope, ParseContext parseResult)
    {
        return await Read(scope, parseResult);
    }
    /// <summary>
    /// Serializes the specified value of type <typeparamref name="T"/> into YAML.
    /// </summary>
    /// <typeparam name="X">The <see cref="Node"/>.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public abstract void Write<X>(WriteContext<X> context, T value, DataStyle style) where X : Node;

    /// <summary>
    /// Deserializes YAML content into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="stream">The <see cref="IYamlReader"/>.</param>
    /// <param name="parseResult">The <see cref="ParseContext"/>.</param>
    /// <returns>A task yielding the deserialized object of type <typeparamref name="T"/>.</returns>
    public abstract ValueTask<T?> Read(IYamlReader stream, ParseContext parseResult);
    public abstract ValueTask<T?> Read(Scope scope, ParseContext parseResult);
}
