using NexYaml;
using NexYaml.Core;
using NexYaml.Parser;

class ValueScopeFactory
{
    public Scope Parse(ScopeContext context, string val, int indent, string tag)
    {
        var valSpan = val.AsSpan();
        if (valSpan.StartsWith('!') && val != YamlCodes.Null)
        {
            var spaceIdx = valSpan.IndexOf(' ');
            var childTag = valSpan[..spaceIdx].ToString();

            // Skip spaces
            do
            {
                spaceIdx++;
            } while (spaceIdx < valSpan.Length && valSpan[spaceIdx] == ' ');

            // Skip first character
            // Replicated the base logic, but not sure why we would skip the first character of what's after the space though -Eideren
            string rest = spaceIdx + 1 < val.Length ? valSpan[(spaceIdx + 1)..].ToString() : string.Empty;
            return Parse(context, rest, indent, childTag);
        }

        if (IsQuoted(valSpan))
            return new ScalarScope(Unquote(val), indent, context, tag);
        if (valSpan.StartsWith('|'))
            return new ScalarScope(ParseLiteralScalar(context, indent, valSpan[1]), indent, context, tag);
        if (valSpan.StartsWith('{') && valSpan.EndsWith('}'))
            return MappingScope.ParseFlow(context, val, indent, tag);
        if (valSpan.StartsWith('[') && valSpan.EndsWith(']'))
            return SequenceScope.ParseFlow(context, val, indent, tag);

        return new ScalarScope(val, indent, context, tag);
    }

    public Scope Parse(ScopeContext context, int indent, string tag)
    {
        if (context.Reader.Move(out var val))
        {
            return Parse(context, val.Trim(), indent, tag);
        }
        throw new EndOfStreamException();
    }

    public Scope ParseFlow(ScopeContext context, string value, int indent, string tag) => throw new NotSupportedException();
    protected string ParseLiteralScalar(ScopeContext context, int indent, char chompHint)
    {
        var sb = new System.Text.StringBuilder();

        while (context.Reader.Peek(out var next))
        {
            int lineIndent = CountIndent(next);
            if (lineIndent < indent)
                break;

            context.Reader.Move();

            if (lineIndent >= indent)
                sb.Append(next.AsSpan()[indent..]);
            else
                sb.Append(next);

            sb.Append('\n'); // normalize to LF
        }

        // Apply chomping
        if (chompHint == '+')
        {
            // Strip exactly one trailing newline if present
            if (sb[^1] == '\n')
            {
                sb.Remove(sb.Length - 1, 1);
            }
        }

        return sb.ToString();
    }

    protected static int CountIndent(string line)
    {
        int i = 0;
        while (i < line.Length && line[i] == ' ')
            i++;
        return i;
    }

    protected static void ExtractTag(ref ReadOnlySpan<char> itemSpan, ref string childTag)
    {
        if (!itemSpan.IsEmpty && itemSpan[0] == '!' && !itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
        {
            int spaceIdx = itemSpan.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                childTag = itemSpan[..spaceIdx].ToString();
                itemSpan = itemSpan[(spaceIdx + 1)..].Trim();
            }
            else
            {
                childTag = itemSpan.ToString();
                itemSpan = ReadOnlySpan<char>.Empty;
            }
        }
    }
    protected static bool IsQuoted(ReadOnlySpan<char> s)
    {
        return s.Length >= 2 &&
               ((s[0] == '\"' && s[^1] == '\"') ||
                (s[0] == '\'' && s[^1] == '\''));
    }
    protected static string Unquote(string s)
    {
        return IsQuoted(s) ? s.Substring(1, s.Length - 2) : s;
    }
    protected static string Unquote(ReadOnlySpan<char> s)
    {
        return IsQuoted(s) ? s.Slice(1, s.Length - 2).ToString() : s.ToString();
    }
}
