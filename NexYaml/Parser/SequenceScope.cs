using System.Collections;

namespace NexYaml.Parser
{
    public sealed class SequenceScope : Scope, IEnumerable<Scope>
    {
        private Scope[] _items;
        private int _count;

        public SequenceScope(int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
            : base(tag, indent, context)
        {
            _items = new Scope[initialCapacity];
            _count = 0;
        }

        public override ScopeKind Kind => ScopeKind.Sequence;

        public int Count => _count;

        public void Add(Scope value)
        {
            if (_count == _items.Length)
            {
                // grow if needed; double the size
                var newArr = new Scope[_items.Length * 2];
                Array.Copy(_items, newArr, _count);
                _items = newArr;
            }

            _items[_count++] = value;
        }

        public Scope this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
        }

        public IEnumerator<Scope> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
