using System.Collections;
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
        public ValueTask<T?> Read<T>(T context)
        {
            if (this is ScalarScope scalar && scalar.Value == "!!null")
            {
                return new ValueTask<T?>(default(T));
            }
            if (typeof(T).IsArray)
            {
                var t = typeof(T).GetElementType()!;
                var arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(t);
                var arraySerializer = (YamlSerializer)Activator.CreateInstance(arraySerializerType)!;

                var value = Convert<T>(arraySerializer.ReadUnknown(this, context));
                return value;
            }
            Type type = typeof(T);

            if (Tag.SequenceEqual("!!ref"))
            {
                var refScalar = this.As<ScalarScope>();
                if (Guid.TryParse(refScalar.Value, out var id))
                {
                    return Context.IdentifiableResolver.AsyncGetRef<T?>(id);
                }
                else
                {
                    throw new InvalidCastException($"couldnt cast ref {refScalar.Value}");
                }
            }

            ValueTask<T?> result;
            if (type.IsInterface || type.IsAbstract || type.IsGenericType)
            {
                YamlSerializer? serializer;
                if (Tag.IsNullOrEmpty())
                {
                    var formatt = Context.Resolver.GetSerializer<T>();
                    result = formatt.Read(this, context);
                }
                else
                {
                    Type alias = Context.Resolver.GetAliasType(Tag);
                    serializer = Context.Resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(this, context);
                    result = Convert<T>(res);
                }
            }
            else
            {
                result = Context.Resolver.GetSerializer<T?>().Read(this, context);
            }
            return result;
        }
        private static async ValueTask<T?> Convert<T>(ValueTask<object?> task)
        {
            return (T?)(await task);
        }
        protected static string ParseLiteralScalar(ScopeContext context, int indent, char chompHint)
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

        public static void ExtractTag(ref ReadOnlySpan<char> itemSpan, ref string childTag)
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
        public static bool IsQuoted(string s)
        {
            return s.Length >= 2 &&
                   ((s.StartsWith('\"') && s.EndsWith('\"')) ||
                    (s.StartsWith('\'') && s.EndsWith('\'')));
        }
        public static bool IsQuoted(ReadOnlySpan<char> s)
        {
            return s.Length >= 2 &&
                   ((s[0] == '\"' && s[s.Length - 1] == '\"') ||
                    (s[0] == '\'' && s[s.Length - 1] == '\''));
        }
        public static string Unquote(string s)
        {
            return IsQuoted(s) ? s.Substring(1, s.Length - 2) : s;
        }
        public static ReadOnlySpan<char> Unquote(ReadOnlySpan<char> s)
        {
            return IsQuoted(s) ? s.Slice(1, s.Length - 2) : s;
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
}
