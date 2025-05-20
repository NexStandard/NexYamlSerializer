using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Plugins;
using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Unicode;

namespace NexYaml;
public class YamlReader(YamlParser parser, IYamlSerializerResolver Resolver) : IYamlReader
{
    public bool HasKeyMapping => parser.HasKeyMapping;
    public bool HasSequence => parser.HasSequence;
    public Marker CurrentMarker => parser.CurrentMark;
    public Dictionary<Guid, List<Action<object>>> ReferenceResolvingMap { get; } = new();

    private Dictionary<Guid, (TaskCompletionSource<object>? tcs, object? result)> _identifiables = new();

    public HashSet<IIdentifiable> Identifiables { get; } = new();

    private List<IResolvePlugin> plugins =
    [
        new NullPlugin(),
        new NullablePlugin(),
        new ArrayPlugin(),
        new ReferencePlugin(),
    ];

    public void Dispose()
    {
        parser.Dispose();
    }

    public bool HasMapping(out ReadOnlySpan<byte> mappingKey)
    {
        return parser.HasMapping(out mappingKey);
    }
    public bool HasMapping(out byte[] mappingKey, bool proxy)
    {
        var x = parser.HasMapping(out var map);
        mappingKey = map.ToArray();
        return x;
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

    public ValueTask<T?> Read<T>(ParseContext parseResult)
    {
        ValueTask<T?> result = default;
        if (IsNullScalar())
        {
            Move();
            return new ValueTask<T?>(default(T));
        }
        Type type = typeof(T);

        foreach (var syntax in plugins)
        {
            if (syntax.Read<T>(this, out var t, parseResult))
            {
                // TODO
                return t;
            }
        }

        // await reference
        if (TryGetCurrentTag(out var tag1))
        {
            var handle = tag1.Handle;

            if (handle == "ref")
            {
                Guid? id = null;
                TryGetScalarAsString(out var idScalar);

                Move(ParseEventType.Scalar);
                if (idScalar != null)
                {
                    return AsyncGetRef<T?>(Guid.Parse(idScalar));
                }
            }
        }


        if (type.IsInterface || type.IsAbstract || type.IsGenericType)
        {
            TryGetCurrentTag(out var tag);
            YamlSerializer? serializer;
            if (tag == null)
            {
                var formatt = Resolver.GetSerializer<T>();
                result = formatt.Read(this, parseResult);
            }
            else
            {
                Type alias = Resolver.GetAliasType(tag.Handle);
                serializer = Resolver.GetSerializer(alias, type);

                var res = serializer.ReadUnknown(this, parseResult);
                result = Convert<T>(res);
            }
        }
        else
        {
            result = Resolver.GetSerializer<T?>().Read(this, parseResult);
        }
        return result;
    }
    private async ValueTask<T> Convert<T>(ValueTask<object> t)
    {
        return (T)(await t);
    }

    // For handling anchors, max need it for !TAG &PARENT_ANCHOR 
    /*
    private void Read<T>(YamlSerializer<T> serializer, ref YamlParser parser, ref T value, ref ParseResult parseResult)
    {
        if (parser.TryResolveCurrentAlias<T>(ref parser, out var aliasValue))
        {
            value = aliasValue!;
            return;
        }

        var withAnchor = parser.TryGetCurrentAnchor(out var anchor);

        // serializer.Read(this, ref value, ref parseResult);

        if (withAnchor)
        {
            parser.RegisterAnchor(anchor, value);
        }
        return;
    }
    */

    public void Move(ParseEventType eventType)
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

    public bool TryGetCurrentTag(out Tag tag)
    {
        return parser.TryGetCurrentTag(out tag);
    }

    public void RegisterIdentifiable(Guid guid, IIdentifiable identifiable)
    {
        ref var field = ref CollectionsMarshal.GetValueRefOrAddDefault(_identifiables, guid, out _);
        if (field.tcs is not null) // Something is waiting, notify it;
            field.tcs.SetResult(identifiable);
        else // Nothing was waiting on this one, set the field
            field.result = identifiable;
    }

    public async ValueTask<T> AsyncGetRef<T>(Guid guid)
    {

        (TaskCompletionSource<object>? tcs, object? result) tcs;
        if (_identifiables.TryGetValue(guid, out var value))
        {
            if (value.result is not null)
            {
                return (T)value.result;
            }
            tcs = value;
        }
        else
        {
            tcs = new();
            tcs.tcs = new TaskCompletionSource<object>();
            _identifiables.Add(guid, tcs);
        }
        return (T)(await tcs.tcs.Task);
    }
}
