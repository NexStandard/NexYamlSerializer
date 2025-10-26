using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using NexYaml.Parser;
using NexYaml.Serialization;
using Stride.Core.Extensions;

namespace NexYaml.XParser
{

    public enum ScopeKind { Scalar, Mapping, Sequence }

    public abstract class Scope
    {
        public abstract ScopeKind Kind { get; }
        public string Tag { get; }
        public int Indent { get; }
        private IYamlSerializerResolver _resolver;
        public IdentifiableResolver IdentifiableResolver { get; private set; }
        protected Scope(string tag, int indent,IYamlSerializerResolver resolver, IdentifiableResolver identifiableResolver) => (Tag, Indent, _resolver, IdentifiableResolver) = (tag, indent, resolver, identifiableResolver);
        public ValueTask<T?> Read<T>(Scope scope,ParseContext context)
        {
            if(scope is ScalarScope scalar && scalar.Value == "!!null")
            {
                return new ValueTask<T?>(default(T));
            }
            Type type = typeof(T);

            if (scope.Tag.SequenceEqual("!!ref"))
            {
                var refScalar = scope.As<ScalarScope>();
                if (Guid.TryParse(refScalar.Value, out var id))
                {
                    return scope.IdentifiableResolver.AsyncGetRef<T?>(id);
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
                if (scope.Tag.IsNullOrEmpty())
                {
                    var formatt = _resolver.GetSerializer<T>();
                    result = formatt.Read(this, context);
                }
                else
                {
                    Type alias = _resolver.GetAliasType(scope.Tag);
                    serializer = _resolver.GetSerializer(alias, type);

                    var res = serializer.ReadUnknown(scope, context);
                    result = Convert<T>(res);
                }
            }
            else
            {
                result = _resolver.GetSerializer<T?>().Read(scope, context);
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
            if(scope is T castedScope)
            {
                return castedScope;
            }
            throw new InvalidCastException($"Expected: {nameof(T)} but got {scope.Kind}");
        }
    }

    public sealed class ScalarScope : Scope
    {
        public string Value { get; }
        public ScalarScope(string value, int indent, IYamlSerializerResolver resolver, IdentifiableResolver identifiableResolver, string tag = "") : base(tag, indent, resolver, identifiableResolver) => Value = value;
        public override ScopeKind Kind => ScopeKind.Scalar;
    }

    public sealed class MappingScope : Scope, IEnumerable<KeyValuePair<string, Scope>>
    {
        private readonly List<KeyValuePair<string,Scope>> _entries = new();
        public MappingScope(int indent, IYamlSerializerResolver resolver, IdentifiableResolver identifiableResolver,string tag = "") : base(tag, indent,resolver, identifiableResolver) { }
        public override ScopeKind Kind => ScopeKind.Mapping;
        public void Add(string key, Scope value) => _entries.Add(new KeyValuePair<string,Scope>(key, value));
        public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator() => _entries.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();
    }

    public sealed class SequenceScope : Scope, IEnumerable<Scope>
    {
        private readonly List<Scope> _items = new();
        public SequenceScope(int indent, IYamlSerializerResolver resolver,IdentifiableResolver identifiableResolver, string tag = "") : base(tag, indent, resolver, identifiableResolver) { }
        public override ScopeKind Kind => ScopeKind.Sequence;
        public void Add(Scope value) => _items.Add(value);
        public IEnumerator<Scope> GetEnumerator() => _items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
    public static class YamlDumpExtensions
    {
        public static string Dump(this Scope scope, int indent = 0, bool includeHeader = true)
        {
            var pad = new string(' ', indent);
            string TagSuffix(string tag) => $"({tag})";

            switch (scope)
            {
                case ScalarScope s:
                    return $"{pad}SCALAR{TagSuffix(s.Tag)}({s.Value})";

                case MappingScope m:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}MAPPING{TagSuffix(m.Tag)}");
                        sb.AppendLine($"{pad}{{");
                        foreach (var (key, val) in m)
                        {
                            if (val is ScalarScope scalar)
                            {
                                sb.AppendLine($"{pad}  SCALAR({key}) = SCALAR{TagSuffix(scalar.Tag)}({scalar.Value})");
                            }
                            else
                            {
                                var header = val.Kind.ToString().ToUpper();
                                sb.AppendLine($"{pad}  SCALAR({key}) = {header}{TagSuffix(val.Tag)}");
                                sb.Append(val.Dump(indent + 2, includeHeader: false));
                            }
                        }
                        sb.AppendLine($"{pad}}}");
                        return sb.ToString();
                    }

                case SequenceScope seq:
                    {
                        var sb = new StringBuilder();
                        if (includeHeader)
                            sb.AppendLine($"{pad}SEQUENCE{TagSuffix(seq.Tag)}");
                        sb.AppendLine($"{pad}[");
                        foreach (var item in seq)
                        {
                            sb.AppendLine(item.Dump(indent + 2, includeHeader: true));
                        }
                        sb.AppendLine($"{pad}]");
                        return sb.ToString();
                    }

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
