using System.Collections;

namespace NexYaml.Parser
{
    public sealed class MappingScope : Scope, IEnumerable<KeyValuePair<string, Scope>>
    {
        // Generously sized backing array to avoid repeated allocations
        private KeyValuePair<string, Scope>[] _entries;
        private int _count;

        public MappingScope(int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
            : base(tag, indent, context)
        {
            _entries = new KeyValuePair<string, Scope>[initialCapacity];
            _count = 0;
        }

        public override ScopeKind Kind => ScopeKind.Mapping;

        public void Add(string key, Scope value)
        {
            if (_count == _entries.Length)
            {
                // If we ever exceed the initial capacity, just double it
                var newArr = new KeyValuePair<string, Scope>[_entries.Length * 2];
                Array.Copy(_entries, newArr, _count);
                _entries = newArr;
            }

            _entries[_count++] = new KeyValuePair<string, Scope>(key, value);
        }

        public KeyValuePair<string, Scope> this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _entries[index];
            }
        }

        public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _entries[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
