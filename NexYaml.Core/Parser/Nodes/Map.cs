namespace NexYaml.Parser.Scopes
{
    public ref struct Map
    {
        public ReadOnlySpan<char> Key;
        public Scope Value;
        public ReadOnlySpan<char> Tag;
    }
}

