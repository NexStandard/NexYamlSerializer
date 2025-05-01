using NexYaml.Parser;
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
    public abstract void Write(IYamlWriter stream, object value, DataStyle style);
    public abstract void Write(IYamlWriter stream, object value);
    public abstract void ReadUnknown(IYamlReader stream, ref object? value, ref ParseResult parseResult);
    public abstract ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext<object?> parseResult);

}
public abstract class YamlSerializer<T> : YamlSerializer
{
    public override void Write(IYamlWriter stream, object value)
    {
        Write(stream, (T)value, Style);
    }
    public override void Write(IYamlWriter stream, object value, DataStyle style)
    {
        Write(stream, (T)value, style);
    }
    public override void ReadUnknown(IYamlReader stream, ref object? value, ref ParseResult parseResult)
    {
        var val = (T?)value!;
        Read(stream, ref val, ref parseResult);
        value = val;
    }
    public abstract void Write(IYamlWriter stream, T value, DataStyle style);
    public abstract void Read(IYamlReader stream, ref T value, ref ParseResult parseResult);
    public override async ValueTask<object?> ReadUnknown(IYamlReader stream, ParseContext<object?> parseResult)
    {
        return await Read(stream, (ParseContext<T>)(object)parseResult);
    }
    public virtual ValueTask<T?> Read(IYamlReader stream, ParseContext<T> parseResult)
    {
        return new ValueTask<T?>();
    }
}
