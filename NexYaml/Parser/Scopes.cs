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

            var sb = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '\\' && i + 1 < input.Length)
                {
                    char next = input[++i];
                    switch (next)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        default:
                            // Unknown escape, keep literally
                            sb.Append(next);
                            break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
    }


    public sealed class MappingScope : Scope, IEnumerable<KeyValuePair<string, Scope>>
    {
        private readonly List<KeyValuePair<string, Scope>> _entries = new();
        public MappingScope(int indent, ScopeContext context, string tag = "") : base(tag, indent, context) { }
        public override ScopeKind Kind => ScopeKind.Mapping;
        public void Add(string key, Scope value) => _entries.Add(new KeyValuePair<string, Scope>(key, value));
        public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
    }

    public sealed class SequenceScope : Scope, IEnumerable<Scope>
    {
        private readonly List<Scope> _items = new();
        public SequenceScope(int indent, ScopeContext context, string tag = "") : base(tag, indent, context) { }
        public override ScopeKind Kind => ScopeKind.Sequence;
        public void Add(Scope value) => _items.Add(value);
        public IEnumerator<Scope> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
