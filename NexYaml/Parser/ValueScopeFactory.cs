using NexYaml;
using NexYaml.Parser;

class ValueScopeFactory
{
    public Scope Parse(ScopeContext context, string val, int indent, string tag)
    {
        if (val.StartsWith('!') && val != "!!null")
        {
            var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string childTag = segs[0];
            string rest = segs.Length > 1 ? segs[1].Trim() : string.Empty;
            return Parse(context, rest, indent, childTag);
        }

        if (IsQuoted(val))
            return new ScalarScope(Unquote(val), indent, context, tag);
        if (val.StartsWith('|'))
            return new ScalarScope(ParseLiteralScalar(context, indent, val[1]), indent, context, tag);
        if (val.StartsWith('{') && val.EndsWith('}'))
            return MappingScope.ParseFlow(context, val, indent, tag);
        if (val.StartsWith('[') && val.EndsWith(']'))
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

            string content = lineIndent >= indent
                ? next.Substring(indent)
                : next;

            sb.Append(content);
            sb.Append('\n'); // normalize to LF
        }

        string result = sb.ToString();

        // Apply chomping
        if (chompHint == '+')
        {
            // Strip exactly one trailing newline if present
            if (result.EndsWith('\n'))
                result = result.Substring(0, result.Length - 1);
        }

        return result;
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
        if (!itemSpan.IsEmpty && itemSpan[0] == '!' && !itemSpan.SequenceEqual("!!null".AsSpan()))
        {
            int spaceIdx = itemSpan.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                childTag = itemSpan.Slice(0, spaceIdx).ToString();
                itemSpan = itemSpan.Slice(spaceIdx + 1).Trim();
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
               ((s[0] == '\"' && s[s.Length - 1] == '\"') ||
                (s[0] == '\'' && s[s.Length - 1] == '\''));
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
