using NexYaml.Parser;
using NexYaml.Serialization;
using Silk.NET.OpenXR;
using Stride.Core;
using System.Diagnostics.CodeAnalysis;

namespace NexYaml;
/// <summary>
/// Provides methods for serializing/deserializing objects to YAML format using the specified <see cref="YamlSerializerOptions"/>.
/// </summary>
public abstract class YamlSerializer
{
    protected virtual DataStyle Style { get; } = DataStyle.Any;
    public abstract void Write<X>(WriteContext<X> context, object value, DataStyle style)
        where X : Node;
    public abstract void Write<X>(WriteContext<X> context, object value)
        where X : Node;
    public abstract ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext parseResult);
}
public abstract class YamlSerializer<T> : YamlSerializer
{
    public override void Write<X>(WriteContext<X> context, object value)
    {
        var x = typeof(T).GetType().Name;
        Write(context,(T)value, Style);
    }
    public override void Write<X>(WriteContext<X> context, object value, DataStyle style)
    {
        Write(context, (T)value, style);
    }

    public abstract void Write<X>(WriteContext<X> context, T value, DataStyle style)
     where X : Node;

    public override async ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext parseResult)
    {
        return await Read(stream, parseResult);
    }
    public abstract ValueTask<T?> Read(IYamlReader stream, ParseContext parseResult);
}
