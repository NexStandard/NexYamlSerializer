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
            Value = DecodeEscapes(value);
        }

        public override ScopeKind Kind => ScopeKind.Scalar;

        private static string DecodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace("\\n", "\n").Replace("\r\n", "\n");
        }
        public static Scope Parse(ScopeContext context, string val, int indent, string tag)
        {
            var valSpan = val.AsSpan();

            if (TryGetQuotedText(valSpan, out var unquotedSpan))
                return new ScalarScope(unquotedSpan.ToString(), indent, context, tag);

            return new ScalarScope(val, indent, context, tag);
        }

        public static Scope Parse(ScopeContext context, int indent, string tag)
        {
            if (context.Reader.Move(out var val))
            {
                return Parse(context, val.Trim(), indent, tag);
            }
            throw new EndOfStreamException();
        }
    }
}
