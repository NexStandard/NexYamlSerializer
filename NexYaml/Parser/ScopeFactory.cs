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

    protected static string Unquote(string s)
    {
        return IsQuoted(s) ? s.Substring(1, s.Length - 2) : s;
    }

    protected static int CountIndent(string line)
    {
        var span = line.AsSpan();
        int i = 0;
        while (i < span.Length && span[i] == ' ')
            i++;
        return i;
    }
    protected static IEnumerable<string> SplitFlowItems(string input)
    {
        var result = new List<string>();
        var sb = new StringBuilder();
        int depth = 0;
        bool inQuotes = false;
        bool inTag = false;
        char quoteChar = '\0';

        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            if (inQuotes)
            {
                sb.Append(c);
                if (c == quoteChar) inQuotes = false;
                continue;
            }

            if (c == '"' || c == '\'')
            {
                inQuotes = true;
                quoteChar = c;
                sb.Append(c);
                continue;
            }
            if (c == '!' && sb.ToString().Trim().Length == 0)
            {
                inTag = true;
            }
            if ((c == ' ' || c == '[' || c == '{') && inTag)
            {
                inTag = false;
                result.Add(sb.ToString().Trim());
                sb.Clear();
                continue;
            }
            if (c == '[' || c == '{') depth++;
            if (c == ']' || c == '}') depth--;

            if (c == ',' && depth == 0 && !inTag)
            {
                result.Add(sb.ToString().Trim());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        if (sb.Length > 0)
            result.Add(sb.ToString().Trim());

        return result;
    }
}
