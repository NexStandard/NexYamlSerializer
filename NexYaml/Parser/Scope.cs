using System.Collections;
using NexYaml.Core;
using NexYaml.Serializers;
using Stride.Core.Extensions;

namespace NexYaml.Parser
{
    /// <summary>
    /// Represents the kind of a YAML scope node.
    /// </summary>
    public enum ScopeKind
    {
        /// <summary>
        /// A scalar value (string, number, boolean, or null).
        /// </summary>
        Scalar,

        /// <summary>
        /// A mapping (key–value pairs, equivalent to a dictionary/object).
        /// </summary>
        Mapping,

        /// <summary>
        /// A sequence (an ordered list of items).
        /// </summary>
        Sequence
    }
    public struct ScopeEnumerator : IEnumerable<Scope>
    {
        public IEnumerator<Scope> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Base class for all YAML <see cref="ScopeKind"/>.
    /// </summary>
    public abstract class Scope
    {
        /// <summary>
        /// Gets the <see cref="ScopeKind"/> of this <see cref="Scope"/>.
        /// </summary>
        public abstract ScopeKind Kind { get; }

        /// <summary>
        /// Gets the YAML tag associated with this scope, if any.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Gets or sets the indentation level of this scope in the source document.
        /// </summary>
        public int Indent { get; set; }

        /// <summary>
        /// Gets the parsing <see cref="ScopeContext"/> that created this scope.
        /// </summary>
        public ScopeContext Context { get; }

        /// <summary>
        /// Initializes a new <see cref="Scope"/>.
        /// </summary>
        /// <param name="tag">The YAML tag associated with this scope.</param>
        /// <param name="indent">The indentation level of this scope.</param>
        /// <param name="context">The parsing <see cref="ScopeContext"/> that owns this scope.</param>
        protected Scope(string tag, int indent, ScopeContext context)
        {
            Tag = tag;
            Indent = indent;
            Context = context;
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

        public static bool TryGetQuotedText(ReadOnlySpan<char> s, out ReadOnlySpan<char> unquoted)
        {
            if (s.Length >= 2 &&
                ((s[0] == '\"' && s[^1] == '\"') ||
                 (s[0] == '\'' && s[^1] == '\'')))
            {
                unquoted = s.Slice(1, s.Length - 2);
                return true;
            }

            unquoted = default;
            return false;
        }

        public static int CountIndent(string line)
        {
            int i = 0;
            while (i < line.Length && line[i] == ' ')
                i++;
            return i;
        }

        public static IEnumerable<string> SplitFlowItems(string input)
        {
            int depth = 0;
            bool inQuotes = false;
            bool inTag = false;
            char quoteChar = '\0';

            int tokenStart = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                switch (c)
                {
                    case '"' or '\'':
                        if (inQuotes)
                        {
                            if (c == quoteChar)
                                inQuotes = false;
                        }
                        else
                        {
                            inQuotes = true;
                            quoteChar = c;
                        }

                        continue;

                    case '!' when input.AsSpan()[tokenStart .. (i - tokenStart)].Trim().Length == 0:
                        inTag = true;
                        continue;

                    case ' ' or '[' or '{' when inTag:
                        inTag = false;
                        goto YIELD_FROM_TOKEN_TO_I;
                    case ',' when depth == 0 && !inTag:
                        goto YIELD_FROM_TOKEN_TO_I;

                    case '[' or '{':
                        depth++;
                        continue;
                    case ']' or '}':
                        depth--;
                        continue;
                }

                continue;

                YIELD_FROM_TOKEN_TO_I:
                var s = input.AsSpan()[tokenStart .. (i - tokenStart)].Trim();
                if (s.Length > 0)
                    yield return s.ToString();
                tokenStart = i + 1;
            }

            if (tokenStart < input.Length)
            {
                var s = input.AsSpan()[tokenStart..].Trim();
                if (s.Length > 0)
                    yield return s.ToString();
            }
        }
    }
}
