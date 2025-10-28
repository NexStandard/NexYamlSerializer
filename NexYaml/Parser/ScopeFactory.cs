using NexYaml.Parser;

abstract class ScopeFactory<T>
    where T : Scope
{
    public abstract T Parse(ScopeContext context, int indent, string tag);
    public abstract T ParseFlow(ScopeContext context, string value, int indent, string tag);
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
        else if (chompHint == ' ')
        {
            // Clip (default): ensure exactly one trailing newline
            int i = result.Length;
            while (i > 0 && result[i - 1] == '\n') i--;
            result = result.Substring(0, i) + "\n";
        }
        // '+' means preserve as is

        return result;
    }


    protected static bool IsQuoted(string s)
    {
        return s.Length >= 2 &&
               ((s.StartsWith('\"') && s.EndsWith('\"')) ||
                (s.StartsWith('\'') && s.EndsWith('\'')));
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
    protected static int CountIndent(string line)
    {
        int i = 0;
        while (i < line.Length && line[i] == ' ')
            i++;
        return i;
    }
    protected static IEnumerable<string> SplitFlowItems(string input)
    {
        int depth = 0;
        bool inQuotes = false;
        bool inTag = false;
        char quoteChar = '\0';

        int tokenStart = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inQuotes)
            {
                if (c == quoteChar) inQuotes = false;
                continue;
            }

            if (c == '"' || c == '\'')
            {
                inQuotes = true;
                quoteChar = c;
                continue;
            }

            if (c == '!' && string.IsNullOrWhiteSpace(input.Substring(tokenStart, i - tokenStart)))
            {
                inTag = true;
            }

            if ((c == ' ' || c == '[' || c == '{') && inTag)
            {
                inTag = false;
                var s = input.Substring(tokenStart, i - tokenStart).Trim();
                if (s.Length > 0)
                    yield return s;
                tokenStart = i + 1;
                continue;
            }

            if (c == '[' || c == '{') depth++;
            if (c == ']' || c == '}') depth--;

            if (c == ',' && depth == 0 && !inTag)
            {
                var s = input.Substring(tokenStart, i - tokenStart).Trim();
                if (s.Length > 0)
                    yield return s;
                tokenStart = i + 1;
            }
        }

        if (tokenStart < input.Length)
        {
            var s = input.Substring(tokenStart).Trim();
            if (s.Length > 0)
                yield return s;
        }
    }
}
