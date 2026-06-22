using System;
using System.Collections.Generic;
using System.Text;
using NexYaml.Core;
using static Stride.Graphics.Buffer;

namespace NexYaml.Parser.Scopes;
public struct Scope
{
    public ScopeKind Kind;
    public int Indent;
    public string Tag;
    public ScopeContext Context;
    private string ScalarValue;
    public string prefix;
    public ReadOnlySpan<char> AsScalar()
    {
        if (Kind is ScopeKind.Scalar)
        {
            return DecodeEscapes(TryGetQuotedText(ConvertStringToSpanUsingUnsafe(ScalarValue)));
        }
        else if(Kind is ScopeKind.NullScalar)
        {
            return YamlCodes.Null.AsSpan();
        }
        else if(Kind is ScopeKind.LazyScalar)
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
    public unsafe Span<char> ConvertStringToSpanUsingUnsafe(string value)
    {
        unsafe
        {
            fixed (char* ptr = value)
            {
                return new Span<char>(ptr, value.Length);
            }
        }
    }
    public unsafe Span<char> ConvertSpanToSpanUsingUnsafe(ReadOnlySpan<char> value)
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
        if(Kind == ScopeKind.FlowSequence)
        {
            return new SequenceEnumerator(this, ScalarValue);
        }
        if(Kind == ScopeKind.BlockSequence)
        {
            return new SequenceEnumerator(this);
        }
        throw new InvalidCastException();
    }
    public MappingEnumerator AsMapping()
    {
        if (Kind == ScopeKind.BlockMapping)
        {
            return new MappingEnumerator(this);
        }
        else if (Kind == ScopeKind.FlowMapping)
        {
            return new MappingEnumerator(this, ScalarValue);
        }
        else if(Kind == ScopeKind.PrefixedBlockMapping)
        {
            return new MappingEnumerator(this, ScalarValue, prefix);
        }
        else
        {
            throw new InvalidCastException();
        }
    }
    public static Scope NewScalar(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.Scalar,
            Tag = tag.ToString(),
            ScalarValue = value.ToString()
        };
    }
    public static Scope NewLazyScalar(int valueIndex, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.LazyScalar,
            Tag = tag.ToString(),
        };
    }
    public static Scope NewNullScalar()
    {
        return new Scope()
        {
            Kind = ScopeKind.NullScalar,
            Tag = string.Empty
        };
    }
    public static Scope NewFlowSequence(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.FlowSequence,
            Tag = tag.ToString(),
            ScalarValue = value.ToString()
        };
    }
    public static Scope NewBlockSequence(int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.BlockSequence,
            Tag = tag.ToString()
        };
    }
    public static Scope NewFlowMapping(ReadOnlySpan<char> value, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.FlowMapping,
            Tag = tag.ToString(),
            ScalarValue = value.ToString()
        };
    }
    public static Scope NewBlockMapping(int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.BlockMapping,
            Tag = tag.ToString()
        };
    }
    public static Scope NewPrefixedBlockMapping(ReadOnlySpan<char> value, string prefix, int indent, ScopeContext context, ReadOnlySpan<char> tag)
    {
        return new Scope()
        {
            Context = context,
            Indent = indent,
            Kind = ScopeKind.PrefixedBlockMapping,
            Tag = tag.ToString(),
            ScalarValue = value.ToString(),
            prefix = prefix
        };
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
