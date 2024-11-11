using System.Diagnostics.CodeAnalysis;

namespace NexYaml.Parser;
public static class ParserExtensions
{
    public delegate void ActionKey(ReadOnlySpan<byte> key);
    public static bool IsNullable(this YamlParser parser, Type value, [MaybeNullWhen(false)] out Type underlyingType)
    {
        return (underlyingType = Nullable.GetUnderlyingType(value)) != null;
    }
    public static void ReadMapping(this IYamlReader stream, ActionKey action)
    {
        stream.ReadWithVerify(ParseEventType.MappingStart);
        while (stream.HasMapping(out var key))
        {
            action(key);
        }
        stream.ReadWithVerify(ParseEventType.MappingEnd);
    }

    public static void ReadSequence(this IYamlReader stream, Action action)
    {
        stream.ReadWithVerify(ParseEventType.SequenceStart);
        while (stream.HasSequence)
        {
            action();
        }
        stream.ReadWithVerify(ParseEventType.SequenceEnd);
    }
}
