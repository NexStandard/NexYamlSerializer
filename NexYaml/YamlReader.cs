using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.Serialization.Formatters;
using Stride.Core;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Unicode;

namespace NexYaml;
internal class YamlReader(YamlParser parser, IYamlFormatterResolver Resolver) : IYamlReader
{
    public bool HasKeyMapping => parser.HasKeyMapping;
    public bool HasSequence => parser.HasSequence;

    public Dictionary<Guid, Action<object>> ReferenceResolvingMap { get; } = new();
    private HashSet<IIdentifiable> identifiables = new();
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

    public bool Move()
    {
        return parser.Read();
    }

    private static readonly Type NullableFormatter = typeof(NullableFormatter<>);
    private byte[] reference = [(byte)'I', (byte)'d'];

    public void AddReference(Guid id, Action<object> resolution)
    {
        if (ReferenceResolvingMap.TryGetValue(id, out var action))
        {
            action += resolution;
            ReferenceResolvingMap[id] = action;
        }
        else
        {
            ReferenceResolvingMap.Add(id, resolution);
        }
    }
    public void Read<T>(ref T? value, ref ParseResult parseResult)
    {
        if (parser.IsNullScalar())
        {
            value = default;
            Move();
            return;
        }
        if(parser.TryGetCurrentTag(out var tag))
        {
            string handle = tag.Handle;
            if(handle == "ref")
            {
                Guid? id = null;
                ParseResult result = default;
                this.TryGetScalarAsString(out var idScalar);

                parser.ReadWithVerify(ParseEventType.Scalar);
                if (idScalar != null)
                {
                    parseResult.IsReference = true;
                    parseResult.Reference = Guid.Parse(idScalar);
                }
                return;
            }
        }
        if (typeof(T).IsArray)
        {
            var t = typeof(T).GetElementType()!;
            object? val = value;
            var arrayFormatterType = typeof(ArrayFormatter<>).MakeGenericType(t);
            var arrayFormatter = (YamlSerializer)Activator.CreateInstance(arrayFormatterType)!;

            arrayFormatter.Read(this, ref val, ref parseResult);
            value = (T)val!;
            return;
        }
        if(typeof(T) == typeof(Type))
        {
            object? val = value;
            TypeFormatter.Instance.Read(this, ref val, ref parseResult);
            value = (T)val!;
            return;
        }
        var type = typeof(T);
        if (parser.IsNullable(type, out var underlyingType))
        {
            var genericFilledFormatter = NullableFormatter.MakeGenericType(underlyingType);
            // TODO : Nullable makes sense?
            var f = (YamlSerializer<T?>?)Activator.CreateInstance(genericFilledFormatter)!;
            f.Read(this, ref value, ref parseResult);
            return;
        }
        else if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            
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

                    Read(formatt, ref parser, ref value, ref parseResult);
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
            formatter.Read(this, ref valueObject, ref parseResult);
            value = (T?)valueObject;
        }
        else
        {
            Resolver.GetFormatter<T>().Read(this, ref value!, ref parseResult);
        }
        if(value is IIdentifiable identifiable and not null)
        {
            identifiables.Add(identifiable);
        }
    }
    public void ResolveReferences()
    {
        foreach(var identifiable in identifiables)
        {
            if(ReferenceResolvingMap.TryGetValue(identifiable.Id,out var value))
            {
                var x = value.GetInvocationList();
                value(identifiable);
            }
        }
    }
    private void Read<T>(YamlSerializer<T> innerFormatter, ref YamlParser parser, ref T value, ref ParseResult parseResult)
    {
        if (parser.TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        innerFormatter.Read(this, ref value, ref parseResult);

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
    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey, ref ParseResult parseResult)
    {
        if (key.SequenceEqual(mappingKey))
        {
            Move();
            Read(ref target, ref parseResult);
            return true;
        }
        return false;
    }

    public bool TryRead<T>(ref T? target, ref ReadOnlySpan<byte> key, byte[] mappingKey)
    {
        var parseResult = new ParseResult();
        if (key.SequenceEqual(mappingKey))
        {
            Move();
            Read(ref target, ref parseResult);
            return true;
        }
        return false;
    }
    public void SkipAfter(ParseEventType eventType)
    {
        parser.SkipAfter(eventType);
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

    public void Read<T>(ref T? value)
    {
        var parseResult = new ParseResult();
        Read(ref value, ref parseResult);
    }
}
