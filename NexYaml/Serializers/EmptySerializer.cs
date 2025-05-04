using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core;

namespace NexYaml.Serializers;
public class EmptySerializer<T> : YamlSerializer<T>
{
    public static YamlSerializer<T> EmptyS()
    {
        return new EmptySerializer<T>();
    }

    public override WriteContext Write(IYamlWriter stream, T value, DataStyle style, in WriteContext context)
    {
        return context.WriteScalar(YamlCodes.Null);
    }

    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        value = default!;
    }
}
