using NexYaml.Core;
using NexYaml.Core.Parser;
using NexYaml.Serialization;

namespace NexYaml.Parser.Scopes;

public struct Scope
{
    public ScopeKind Kind;
    public ScopeContext Context;
    public string ScalarValue;
    public MapStrategy? mapStrategy;
    public SequenceStrategy? sequenceStrategy;
    public ReadOnlySpan<char> AsScalar()
    {
        if (Kind is ScopeKind.Scalar)
        {
            return DecodeEscapes(TryGetQuotedText(ConvertStringToSpanUsingUnsafe(ScalarValue)));
        }
        else if (Kind is ScopeKind.NullScalar)
        {
            return YamlCodes.Null.AsSpan();
        }
        else if (Kind is ScopeKind.LazyScalar)
        {
            Context.Reader.Move(out var currentLine);
            int colonIdx = currentLine.IndexOf(':');
            if (colonIdx < 0)
                throw new InvalidOperationException($"Invalid mapping line: '{currentLine}'");

            ReadOnlySpan<char> valSpan = colonIdx + 1 < currentLine.Length
                ? currentLine.Slice(colonIdx + 1).Trim()
                : ReadOnlySpan<char>.Empty;
            if (valSpan[0] == '!')
            {
                colonIdx = valSpan.IndexOf(' ');
            }
            else
            {
                colonIdx = 0;
            }
            return DecodeEscapes(TryGetQuotedText(ConvertSpanToSpanUsingUnsafe(valSpan.Slice(colonIdx))));
        }
        throw new InvalidCastException();
    }
    public bool IsNull
    {
        get
        {
            return Kind is ScopeKind.NullScalar;
        }
    }

    public int Indent { get; init; }

    public bool IsSequence => Kind == ScopeKind.BlockSequence || Kind == ScopeKind.FlowSequence;

    public bool IsScalar => Kind == ScopeKind.Scalar || Kind == ScopeKind.LazyScalar || Kind == ScopeKind.NullScalar;

    public bool IsMapping => Kind == ScopeKind.BlockMapping || Kind == ScopeKind.FlowMapping || Kind == ScopeKind.PrefixedBlockMapping;

    public IYamlSerializerResolver Resolver => Context.Resolver;

    public IdentifiableResolver IdentifiableResolver => Context.IdentifiableResolver;

    private Span<char> ConvertStringToSpanUsingUnsafe(string value)
    {
        unsafe
        {
            fixed (char* ptr = value)
            {
                return new Span<char>(ptr, value.Length);
            }
        }
    }
    private Span<char> ConvertSpanToSpanUsingUnsafe(ReadOnlySpan<char> value)
    {
        unsafe
        {
            fixed (char* ptr = value)
            {
                return new Span<char>(ptr, value.Length);
            }
        }
    }
    public SequenceEnumerator AsSequence()
    {
        if(sequenceStrategy is not null)
        {
            return new SequenceEnumerator(sequenceStrategy, this);
        }
        throw new InvalidCastException(Kind.ToString());
    }
    public MappingEnumerator AsMapping()
    {
        if(mapStrategy is not null)
        {
            return new MappingEnumerator(mapStrategy, this);
        }
        throw new InvalidCastException();
    }
    public static ReadOnlySpan<char> DecodeEscapes(Span<char> buffer)
    {
        int write = 0;

        for (int read = 0; read < buffer.Length; read++)
        {
            char c = buffer[read];

            // \n
            if (c == '\\' && read + 1 < buffer.Length && buffer[read + 1] == 'n')
            {
                buffer[write++] = '\n';
                read++;
                continue;
            }

            // \r\n
            if (c == '\r' && read + 1 < buffer.Length && buffer[read + 1] == '\n')
            {
                buffer[write++] = '\n';
                read++;
                continue;
            }

            buffer[write++] = c;
        }

        return buffer[..write];
    }
    private static Span<char> TryGetQuotedText(Span<char> s)
    {
        if (s.Length >= 2 &&
            ((s[0] == '\"' && s[^1] == '\"') ||
             (s[0] == '\'' && s[^1] == '\'')))
        {
            return s.Slice(1, s.Length - 2);
        }
        return s;
    }
}
public ref struct Element
{
    public Scope Data;
    public ReadOnlySpan<char> Tag;
}
public abstract class MapStrategy
{
    public abstract bool MoveNext(out Map map, Scope data);
}
public abstract class SequenceStrategy
{
    public abstract bool MoveNext(out Element scope, Scope data);
}
public ref struct MappingEnumerator(MapStrategy strategy, Scope data)
{
    public Map Current { get; private set; }
    public MappingEnumerator GetEnumerator() => this;
    public bool MoveNext()
    {
        Map tmp;
        var result = strategy.MoveNext(out tmp, data);
        Current = tmp;
        return result;
    }
}
public ref struct SequenceEnumerator(SequenceStrategy strategy, Scope data)
{
    public Element Current { get; private set; }
    public SequenceEnumerator GetEnumerator() => this;
    public bool MoveNext()
    {
        Element tmp;
        var result = strategy.MoveNext(out tmp, data);
        Current = tmp;
        return result;
    }
}