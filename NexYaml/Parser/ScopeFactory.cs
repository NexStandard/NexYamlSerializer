using System.Text;
using NexYaml.Parser;

abstract class ScopeFactory<T>
    where T : Scope
{
    public abstract T Parse(ScopeContext context,int indent, string tag);
    public abstract T ParseFlow(ScopeContext context,string value, int indent, string tag);
    protected string ParseLiteralScalar(int indent)
    {
        throw new NotImplementedException();
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
               ((s[0] == '\"' && s[s.Length-1] == '\"') ||
                (s[0] == '\'' && s[s.Length-1] == '\''));
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
