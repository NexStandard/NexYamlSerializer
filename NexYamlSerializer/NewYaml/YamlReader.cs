using NexVYaml;
using NexVYaml.Parser;
using NexVYaml.Serialization;
using NexYaml.Core;
using Stride.Core;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexVYaml.Parser;
internal class YamlReader(YamlParser parser, IYamlFormatterResolver Resolver) : IYamlReader
{
    public bool HasKeyMapping => parser.HasKeyMapping;
    public bool HasSequence => parser.HasSequence;

    public void Dispose()
    {
        parser.Dispose();
    }

    public bool HasMapping(out ReadOnlySpan<byte> mappingKey)
    {
        return parser.HasMapping(out mappingKey);
    }

    public bool IsNullScalar()
    { 
        return parser.IsNullScalar();
    }

    public void Read(ref ReadOnlySpan<byte> span)
    {
        parser.TryGetScalarAsSpan(out span);
    }

    public void Read(ref string? value)
    {
        if (parser.TryGetScalarAsSpan(out var span))
        {
            value = StringEncoding.Utf8.GetString(span);
            parser.ReadWithVerify(ParseEventType.Scalar);
            return;
        }
        value = null;
    }

    public void Read(ref int value)
    {
        throw new NotImplementedException();
    }

    public void Read(ref float value)
    {
        throw new NotImplementedException();
    }
    public bool Read()
    {
        return parser.Read();
    }
    static readonly Type NullableFormatter = typeof(NullableFormatter<>);

    public void Read<T>(ref T? value)
    {
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Deserialize(this, ref val);
            value = (T)val!;
            return;
        }
        var type = typeof(T);
        if (ParserExtensions.IsNullable(parser, type, out var underlyingType))
        {
            var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericFilledFormatter)!;
            f.Deserialize(this, ref value);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            parser.TryGetCurrentTag(out var tag);
            YamlSerializer? formatter;
            if (tag == null)
            {
                var formatt = Resolver.GetGenericFormatter<T>();
                if (formatt == null)
                {
                    value = default;
                    return;
                }

                else
                {

                    Read(formatt, ref parser, ref value);
                    return;
                }
            }
            else
            {
                Type alias;
                // TODO: Problem is that !!null etc gets consumed as Tag on collections instead of a scalar value
                if (parser.IsNullScalar())
                {
                    parser.Read();
                    value = default;
                    return;
                }
                alias = Resolver.GetAliasType(tag.Handle);
                formatter = Resolver.GetFormatter(alias);
                formatter ??= Resolver.GetFormatter(alias, type);
            }
            if (formatter == null)
            {
                value = default;
                return;
            }
            var valueObject = (object)value;
            formatter.Deserialize(this, ref valueObject);
            value = (T?)valueObject;
        }
        else
        {
            Resolver.GetFormatter<T>().Deserialize(this, ref value!);
        }
    }
    void Read<T>(YamlSerializer<T> innerFormatter, ref YamlParser parser, ref T value)
    {
        if (parser.TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        innerFormatter.Deserialize(this, ref value);

        if (withAnchor)
        {
            parser.RegisterAnchor(anchor, value);
        }
        return;
    }

    public void ReadWithVerify(ParseEventType eventType)
    {
        parser.ReadWithVerify(eventType);
    }

    public void Reset()
    {
        parser.Reset();
    }

    public void SkipAfter(ParseEventType eventType)
    {
        parser.SkipAfter(eventType);
    }

    public void SkipCurrentNode()
    {
        parser.SkipCurrentNode();
    }

    public void SkipRead()
    {
        parser.SkipRead();
    }

    public bool TryGetScalarAsSpan([MaybeNullWhen(false)] out ReadOnlySpan<byte> span)
    {
        return parser.TryGetScalarAsSpan(out span);
    }

    public bool TryGetScalarAsString(out string? value)
    {
        return parser.TryGetScalarAsString(out value);
    }
}
