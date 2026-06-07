using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml;
/// <summary>
/// Provides methods for serializing/deserializing objects to YAML format.
/// </summary>
public interface IYamlSerializer
{
    /// <summary>
    /// Gets the preferred <see cref="DataStyle"/>
    /// </summary>
    DataStyle Style { get; }

    /// <summary>
    /// Serializes the specified object into YAML.
    /// </summary>
    /// <typeparam name="X">The <see cref="Node"/> type.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The object to serialize.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    void Write(Node context, object value, DataStyle style);
    ValueTask<object?> ReadUnknown(Scope scope, object? context);
    ValueTask<object?> ReadUnknown(Scope scope);
}
/// <summary>
/// Provides methods for serializing and deserializing objects of type <typeparamref name="T"/> to/from YAML.
/// </summary>
/// <typeparam name="T">The type handled by the serializer.</typeparam>
public interface IYamlSerializer<T> : IYamlSerializer
{
    DataStyle IYamlSerializer.Style => DataStyle.Any;

    void IYamlSerializer.Write(Node context, object value, DataStyle style)
    {
        Write(context, (T)value, style);
    }
    async ValueTask<object?> IYamlSerializer.ReadUnknown(Scope scope, object? parseResult)
    {
        return await Read(scope, (T?)parseResult);
    }
    async ValueTask<object?> IYamlSerializer.ReadUnknown(Scope scope)
    {
        return await Read(scope);
    }

    /// <summary>
    /// Serializes the specified value of type <typeparamref name="T"/> into YAML.
    /// </summary>
    /// <typeparam name="X">The <see cref="Node"/>.</typeparam>
    /// <param name="context">The <see cref="WriteContext{T}"/>.</param>
    /// <param name="value">The value to serialize.</param>
    /// <param name="style">The <see cref="DataStyle"/>.</param>
    public void Write(Node context, T value, DataStyle style);

    public ValueTask<T> Read(Scope scope, T? parseResult = default);
}
