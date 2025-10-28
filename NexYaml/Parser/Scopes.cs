using System.Collections;
using System.Text;
using NexYaml.Parser;
using NexYaml.Serialization;
using NexYaml.Serializers;
using Stride.Core.Extensions;

namespace NexYaml.Parser
{

    public enum ScopeKind { Scalar, Mapping, Sequence }

    public abstract class Scope
    {
        public abstract ScopeKind Kind { get; }
        public string Tag { get; }
        public int Indent { get; set; }
        public ScopeContext Context { get; }
        protected Scope(string tag, int indent, ScopeContext context) => (Tag, indent, Context) = (tag, indent, context);
        public ValueTask<T?> Read<T>(ParseContext context)
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
    }
    public static class YamlScopeExtensions
    {
        public static T As<T>(this Scope scope)
            where T : Scope
        {
            if (scope is T castedScope)
            {
                return castedScope;
            }
            throw new InvalidCastException($"Expected: {typeof(T).Name} but got {scope.Kind}");
        }
    }

    public sealed class ScalarScope : Scope
    {
        public string Value { get; }

        public ScalarScope(
            string value,
            int indent,
            ScopeContext context,
            string tag = ""
        ) : base(tag, indent,context)
        {
            Value = DecodeEscapes(value);
        }

        public override ScopeKind Kind => ScopeKind.Scalar;

        private static string DecodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.Replace("\\n", "\n").Replace("\\\"", "\"");
        }
    }


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

        public int Count => _count;

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
