using NexYaml.Parser;
using NexYaml.ResolvePlugin;
using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Unicode;

namespace NexYaml;
public class YamlReader(YamlParser parser, IYamlSerializerResolver Resolver) : IYamlReader
{
    public bool HasKeyMapping => parser.HasKeyMapping;
    public bool HasSequence => parser.HasSequence;

    public Dictionary<Guid, Action<object>> ReferenceResolvingMap { get; } = new();
    private HashSet<IIdentifiable> identifiables = new();
    private List<IResolvePlugin> plugins =
    [
        new NullPlugin(),
            new DelegatePlugin(),
            new NullablePlugin(),
            new ArrayPlugin(),
            new ReferencePlugin(),
            new TypePlugin(),
    ];
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
        foreach (var syntax in plugins)
        {
            if (syntax.Read(this, ref value, ref parseResult))
            {
                return;
            }
        }
        Type type = typeof(T);
        if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            TryGetCurrentTag(out var tag);
            YamlSerializer? serializer;
            if (tag == null)
            {
                var formatt = Resolver.GetGenericSerializer<T>();
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
                serializer = Resolver.GetSerializer(alias);
                serializer ??= Resolver.GetSerializer(alias, type);
            }
            if (serializer == null)
            {
                value = default;
                return;
            }
            var valueObject = (object)value;
            serializer.Read(this, ref valueObject, ref parseResult);
            value = (T?)valueObject;
        }
        else
        {
            Resolver.GetSerializer<T>().Read(this, ref value!, ref parseResult);
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
    private void Read<T>(YamlSerializer<T> serializer, ref YamlParser parser, ref T value, ref ParseResult parseResult)
    {
        if (parser.TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        serializer.Read(this, ref value, ref parseResult);

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

    public bool TryGetCurrentTag(out Tag tag)
    {
        return parser.TryGetCurrentTag(out tag);
    }
}
