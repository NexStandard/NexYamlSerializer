#nullable enable
using NexVYaml.Emitter;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using Stride.Core;
using System;

namespace NexVYaml;

public interface IYamlFormatter
{
    void IndirectSerialize(ref Utf8YamlEmitter emitter, object value, YamlSerializationContext context, DataStyle style = DataStyle.Normal) { throw new NotImplementedException($"The method {nameof(IndirectSerialize)} isn't implemented on {this.GetType()}"); }
    object? IndirectDeserialize(ref YamlParser parser, YamlDeserializationContext context) { throw new NotImplementedException($"The method {nameof(IndirectSerialize)} isn't implemented on {this.GetType()}"); }
}
/// <summary>
/// Represents a YAML formatter for objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of objects to be serialized and deserialized.</typeparam>
public interface IYamlFormatter<T> : IYamlFormatter
{
    /// <summary>
    /// Serializes the provided value of type <typeparamref name="T"/> into a YAML representation.
    /// </summary>
    /// <param name="emitter">The <see cref="Utf8YamlEmitter"/> to write YAML data to.</param>
    /// <param name="value">The value of type <typeparamref name="T"/> to be serialized.</param>
    /// <param name="context">The <see cref="YamlSerializationContext"/> providing serialization context.</param>
    void Serialize(ref Utf8YamlEmitter emitter, T value, YamlSerializationContext context, DataStyle style = DataStyle.Any);

    /// <summary>
    /// Deserializes a YAML representation from the provided <see cref="YamlParser"/> and <see cref="YamlDeserializationContext"/>.
    /// </summary>
    /// <param name="parser">The <see cref="YamlParser"/> containing the YAML data to be deserialized.</param>
    /// <param name="context">The <see cref="YamlDeserializationContext"/> providing deserialization context.</param>
    /// <returns>The deserialized value of type <typeparamref name="T"/>.</returns>
    T? Deserialize(ref YamlParser parser, YamlDeserializationContext context);
}
public abstract class YamlSerializer2
{
    public abstract void Serialize(ref IYamlStream emitter, object value, DataStyle style = DataStyle.Normal);
    // object? Read(ref YamlParser parser, YamlDeserializationContext context) { throw new NotImplementedException($"The method {nameof(IndirectSerialize)} isn't implemented on {this.GetType()}"); }

}

public abstract class YamlSerializer2<T> : YamlSerializer2
{
    public override void Serialize(ref IYamlStream stream, object value,  DataStyle style = DataStyle.Normal)
    {
        Serialize(ref stream, (T)stream, style);
    }
    public abstract void Serialize(ref IYamlStream stream, T value, DataStyle style = DataStyle.Normal);
}