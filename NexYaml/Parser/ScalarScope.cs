using NexYaml.Core;

namespace NexYaml.Parser
{
    public sealed class ScalarScope : Scope
    {
        public string Value { get; }

        public ScalarScope(
            string value,
            int indent,
            ScopeContext context,
            string tag
        ) : base(tag, indent, context)
        {
            Value = DecodeEscapes(TryGetQuotedText(value).ToString());
        }

        public override ScopeKind Kind => ScopeKind.Scalar;

        private static string DecodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace("\\n", "\n").Replace("\r\n", "\n");
        }

        private static ReadOnlySpan<char> TryGetQuotedText(ReadOnlySpan<char> s)
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
}
