using NexYaml.Core;
using NexYaml.Serialization;
using Stride.Core.Shaders.Ast;
using Stride.Input;
using static Stride.Graphics.Buffer;

namespace NexYaml.Parser.Scopes;

public ref struct SequenceEnumerator
{
    private int count;
    public Scope Current { get; private set; }
    private string[]? value;
    string bufferedTag = string.Empty;
    Scope data;
    public SequenceEnumerator GetEnumerator() => this;
    public SequenceEnumerator(Scope data,string value)
    {
        this.data = data;
        this.value = ScopeUtils.NewSplitItems(value.Substring(1, value.Length - 2).Trim());
    }
    public SequenceEnumerator(Scope data)
    {
        this.data = data;
    }
    public bool MoveNext()
    {
        if(data.Kind is ScopeKind.FlowSequence)
        {
            return ParseFlowSequence();
        }
        if(data.Kind is ScopeKind.BlockSequence)
        {
            return ParseBlockSequence();
        }
        return false;
    }
    private bool ParseFlowSequence()
    {
        for (var i = count; i < value.Length; i++)
        {
            count = i+1;
            string item = value[i];

            if (item.StartsWith('!') && item != YamlCodes.Null)
            {
                var segs = item.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (segs.Length == 1)
                {
                    bufferedTag = segs[0];
                    continue;
                }
                else
                {
                    bufferedTag = segs[0];
                    item = segs.Length > 1 ? segs[1].Trim() : string.Empty;

                }
            }
            if (item.StartsWith('|'))
            {
                Current = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 2, item[1]), data.Indent + 2, data.Context, bufferedTag);
            }
            else if (item.StartsWith('{') && item.EndsWith('}'))
            {
                Current = Scope.NewFlowMapping(item, data.Indent + 2, data.Context, bufferedTag);
            }
            else if (item.StartsWith('[') && item.EndsWith(']'))
            {
                Current = Scope.NewFlowSequence(item, data.Indent, data.Context, bufferedTag);
            }
            else if (item.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                Current = Scope.NewNullScalar();
            }
            else
            {
                Current = Scope.NewScalar(item, data.Indent + 2, data.Context, bufferedTag);
            }
            return true;
        }
        return false;
    }
    private bool ParseBlockSequence()
    {
        while (data.Context.Reader.Peek(out var next))
        {
            int lineIndent = ScopeUtils.CountIndent(next);
            if (lineIndent != data.Indent)
                return false;

            ReadOnlySpan<char> line = next[lineIndent..];
            if (line.Length == 0 || line[0] != '-')
                return false;

            // consume this line


            // skip the leading '-' and any following spaces
            var itemSpan = line[1..].Trim();

            // tag detection
            ScopeUtils.ExtractTag(ref itemSpan, out var childTag);

            if (itemSpan.Length > 0)
            {
                
                switch (itemSpan[0])
                {
                    // literal block scalar
                    case '|':
                        data.Context.Reader.Move();
                        Current = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, itemSpan[1]), data.Indent + 2, data.Context, childTag);
                        return true;
                    // flow mapping
                    case '{' when itemSpan[^1] == '}':
                        data.Context.Reader.Move();
                        Current = Scope.NewFlowMapping(itemSpan, data.Indent + 2, data.Context, childTag);
                        return true;
                    // flow sequence
                    case '[' when itemSpan[^1] == ']':
                        data.Context.Reader.Move();
                        Current = Scope.NewFlowSequence(itemSpan, data.Indent + 2, data.Context, childTag);
                        return true;
                    // inline mapping (key: value)
                    default:
                        int colonIdx = itemSpan.IndexOf(':');
                        if (colonIdx >= 0)
                        {
                            var keySpan = itemSpan[..colonIdx].Trim();
                            var valSpan = itemSpan[(colonIdx + 1)..].Trim();
                            
                            Current = Scope.NewPrefixedBlockMapping(valSpan, keySpan.ToString(), data.Indent + 2, data.Context, childTag);
                        }
                        else
                        {
                            data.Context.Reader.Move();
                            if (itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
                            {
                                Current = Scope.NewNullScalar();
                            }
                            else
                            {
                                Current = Scope.NewScalar(itemSpan, data.Indent + 2, data.Context, childTag);
                            }
                        }
                        return true;
                }
            }
            else
            {
                data.Context.Reader.Move();
                // empty item, look ahead for nested structure
                if (data.Context.Reader.Peek(out var la))
                {
                    int nextIndent = ScopeUtils.CountIndent(la);
                    var nextTrim = la[nextIndent..];

                    if (nextIndent > data.Indent)
                    {
                        if (nextTrim.Length > 0 && nextTrim[0] == '-')
                        {
                            
                            Current = Scope.NewBlockSequence(data.Indent + 2, data.Context, childTag);

                        }
                        else
                        {
                            Current = Scope.NewBlockMapping(data.Indent + 2, data.Context, childTag);
                        }
                        return true;
                    }
                }
                
                Current = Scope.NewScalar(string.Empty, data.Indent + 2, data.Context, childTag);
                return true;
            }
        }
        return false;
    }
}

public static class ScopeUtils
{
    public static void ExtractTag(ref ReadOnlySpan<char> itemSpan, out ReadOnlySpan<char> childTag)
    {
        if (!itemSpan.IsEmpty && itemSpan[0] == '!' && !itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
        {
            int spaceIdx = itemSpan.IndexOf(' ');
            if (spaceIdx >= 0)
            {
                childTag = itemSpan[..spaceIdx];
                itemSpan = itemSpan[(spaceIdx + 1)..].Trim();
            }
            else
            {
                childTag = itemSpan;
                itemSpan = ReadOnlySpan<char>.Empty;
            }
        }
        else
        {
            childTag = ReadOnlySpan<char>.Empty;
        }
    }
    public static int CountIndent(string line)
    {
        int i = 0;
        while (i < line.Length && line[i] == ' ')
            i++;
        return i;
    }
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

