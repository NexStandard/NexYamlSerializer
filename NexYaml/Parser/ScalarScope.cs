namespace NexYaml.Parser
{
    public sealed class ScalarScope : Scope
    {
        public string Value { get; }

        public ScalarScope(
            string value,
            int indent,
            ScopeContext context,
            string tag = ""
        ) : base(tag, indent, context)
        {
            Value = DecodeEscapes(value);
        }

        public override ScopeKind Kind => ScopeKind.Scalar;

        private static string DecodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace("\\n", "\n").Replace("\r\n","\n");
        }
    }
}
