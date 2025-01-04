using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;

namespace NexYaml.Serializers;
public class EmptySerializer<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS()
    {
        return new EmptySerializer<T>();
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        stream.Write(YamlCodes.NullString);
    }

    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        value = default!;
    }
}
