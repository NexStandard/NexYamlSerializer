using NexYaml.Core;
using NexYaml.Core.Parser;

namespace NexYaml.Parser.Scopes;

public static class ScopeUtils
{
    public static int CountIndent(ReadOnlySpan<char> line)
    {
        int i = 0;
        while (i < line.Length && line[i] == ' ')
            i++;
        return i;
    }
    public static string[] NewSplitItems(string input)
    {
        var items = new List<string>();

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
                if (c == quoteChar)
                    inQuotes = false;
                continue;
            }

            switch (c)
            {
                case '"' or '\'':
                    inQuotes = true;
                    quoteChar = c;
                    break;

                case '!':
                    inTag = true;
                    break;

                case '[' or '{':
                    depth++;
                    break;

                case ']' or '}':
                    depth--;
                    break;

                case ',':
                    // Tag endet hier
                    if (inTag)
                        inTag = false;

                    if (depth == 0)
                    {
                        string s = input.Substring(tokenStart, i - tokenStart).Trim();
                        if (s.Length > 0)
                            items.Add(s);
                        tokenStart = i + 1;
                    }
                    break;
            }
        }

        if (tokenStart < input.Length)
        {
            string s = input.Substring(tokenStart).Trim();
            if (s.Length > 0)
                items.Add(s);
        }

        return items.ToArray();
    }


    public static string ParseLiteralScalar(ScopeContext context, int indent, char chompHint)
    {
        var sb = new System.Text.StringBuilder();

        while (context.Reader.Peek(out var next))
        {
            int lineIndent = CountIndent(next);
            if (lineIndent < indent)
                break;

            context.Reader.Move();

            if (lineIndent >= indent)
                sb.Append(next[indent..]);
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

}

