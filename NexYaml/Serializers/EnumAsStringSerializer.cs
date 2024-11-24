using NexYaml.Core;
using NexYaml.Parser;
using Stride.Core;
using System.Runtime.Serialization;

namespace NexYaml.Serializers;

public class EnumAsStringSerializer<T> : YamlSerializer<T>
    where T : Enum
{
    private static readonly Dictionary<string, T> NameValueMapping;
    private static readonly Dictionary<T, string> ValueNameMapping;

    static EnumAsStringSerializer()
    {
        var names = new List<string>();
        var values = new List<object>();

        var type = typeof(T);
        foreach (var item in type.GetFields().Where(x => x.FieldType == type))
        {
            var value = item.GetValue(null);
            values.Add(value!);

            var attributes = item.GetCustomAttributes(true);
            if (attributes.OfType<EnumMemberAttribute>().FirstOrDefault() is { Value: { } enumMemberValue })
            {
                names.Add(enumMemberValue);
            }
            else
            {
                var name = Enum.GetName(type, value!);
                names.Add(ToCamelCase(name!));
            }
        }

        NameValueMapping = new Dictionary<string, T>(names.Count);
        ValueNameMapping = new Dictionary<T, string>(names.Count);

        foreach (var (value, name) in values.Zip(names, (v, n) => (v, n)))
        {
            NameValueMapping[name] = (T)value;
            ValueNameMapping[(T)value] = name;
        }
    }

    private static string ToCamelCase(string s)
    {
        var span = s.AsSpan();
        if (span.Length <= 0 ||
            span.Length <= 1 && char.IsLower(span[0]))
        {
            return s;
        }

        Span<char> buf = stackalloc char[span.Length];
        buf[0] = char.ToLowerInvariant(span[0]);
        span[1..].CopyTo(buf[1..]);
        return buf.ToString();
    }

    public override void Write(IYamlWriter stream, T value, DataStyle style)
    {
        if (ValueNameMapping.TryGetValue(value, out var name))
        {
            stream.Write(name);
        }
        else
        {
            throw new YamlException($"Cannot detect a value of enum: {typeof(T)}, {value}");
        }
    }

    public override void Read(IYamlReader stream, ref T value, ref ParseResult result)
    {
        if (stream.TryGetScalarAsString(out var scalar))
        {
            if (scalar == null)
            {
                value = default;
            }
            else if (NameValueMapping.TryGetValue(scalar, out var val))
            {
                value = val;
            }
        }

        throw new YamlException($"Cannot detect a scalar value of {typeof(T)}");
    }
}

