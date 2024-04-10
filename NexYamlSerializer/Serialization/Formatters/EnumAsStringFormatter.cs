#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NexVYaml.Internal;
using NexVYaml.Parser;
using Stride.Core;

namespace NexVYaml.Serialization;

public class EnumAsStringFormatter<T> : YamlSerializer<T>,IYamlFormatter<T> where T : Enum
{
    static readonly Dictionary<string, T> NameValueMapping;
    static readonly Dictionary<T, string> ValueNameMapping;

    static EnumAsStringFormatter()
    {
        var names = new List<string>();
        var values = new List<object>();

        var type = typeof(T);
        foreach (var item in type.GetFields().Where(x => x.FieldType == type))
        {
            var value = item.GetValue(null);
            values.Add(value);

            var attributes = item.GetCustomAttributes(true);
            if (attributes.OfType<EnumMemberAttribute>().FirstOrDefault() is { Value: { } enumMemberValue })
            {
                names.Add(enumMemberValue);
            }
            else
            {
                var name = Enum.GetName(type, value);
                names.Add(ToCamelCase(name));
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
            (span.Length <= 1 && char.IsLower(span[0])))
        {
            return s;
        }

        Span<char> buf = stackalloc char[span.Length];
        buf[0] = char.ToLowerInvariant(span[0]);
        span[1..].CopyTo(buf[1..]);
        return buf.ToString();
    }
    public override T Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        var scalar = parser.ReadScalarAsString();
        if (scalar is null)
        {
            throw new YamlSerializerException($"Cannot detect a scalar value of {typeof(T)}");
        }

        if (NameValueMapping.TryGetValue(scalar, out var value))
        {
            return value;
        }
        throw new YamlSerializerException($"Cannot detect a scalar value of {typeof(T)}");
    }

    public override void Serialize(ref ISerializationWriter stream, T value, DataStyle style = DataStyle.Normal)
    {
        if (ValueNameMapping.TryGetValue(value, out var name))
        {
            stream.Serialize(ref name);
        }
        else
        {
            throw new YamlSerializerException($"Cannot detect a value of enum: {typeof(T)}, {value}");
        }
    }
}

