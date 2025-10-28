using System.Collections;
using NexYaml.Parser;
using Stride.Input;

namespace NexYaml.Parser
{
    public sealed class MappingScope : Scope, IEnumerable<KeyValuePair<string, Scope>>
    {
        private bool flow;
        private string value;
        private bool startValue;
        private string startString;
        private string startKey;

        internal MappingScope(int nonsense, int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
    : base(tag, indent, context)
        {
        }
        internal MappingScope(int nonsense, string value, int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
: base(tag, indent, context)
        {
            flow = true;
            this.value = value;
        }
        internal MappingScope(int nonsense, string startValue, string startKey, int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
: base(tag, indent, context)
        {
            this.startValue = true;
            this.startString = startValue;
            this.startKey = startKey;
        }
        public override ScopeKind Kind => ScopeKind.Mapping;

        public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator()
        {

            if (flow)
            {
                return new BlockFlowParse(this, value).GetEnumerator();
            }
            else if (startValue)
            {
                return new BlockMappingPrefixedParse(this, startKey, startString).GetEnumerator();
            }
            else
            {
                return new BlockMappingParse(this).GetEnumerator();
            }

        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static MappingScope Parse(ScopeContext context, int indent, string tag)
        {
            var map = new MappingScope(1, indent, context, tag);
            return map;
        }
        public static MappingScope ParseFlow(ScopeContext context, string value, int indent, string tag)
        {
            return new MappingScope(1, value, indent, context, tag);
        }
        public KeyValuePair<string, Scope> StandardMappingResolve(ScopeContext context, MappingScope map, string key, string val, string childTag)
        {
            if (IsQuoted(val))
                return new(key, new ScalarScope(Unquote(val), map.Indent + 2, context, childTag));
            else if (val.StartsWith('|'))
                return new(key, new ScalarScope(ParseLiteralScalar(map.Context, map.Indent + 1, val[1]), map.Indent + 2, context, childTag));
            else if (val.StartsWith('{') && val.EndsWith("}"))
                return new(key, MappingScope.ParseFlow(context, val, map.Indent + 2, childTag));
            else if (val.StartsWith('[') && val.EndsWith("]"))
                return new(key, SequenceScope.ParseFlow(context, val, map.Indent + 2, childTag));
            else
                return new(key, new ScalarScope(val, map.Indent + 2, context, childTag));
        }
 
        public KeyValuePair<string, Scope> StandardMappingResolve(MappingScope map, string key, string val, string childTag)
        {
            if (IsQuoted(val))
                return new(key, new ScalarScope(Unquote(val), map.Indent + 2, map.Context, childTag));
            else if (val.StartsWith('|'))
                return new(key, new ScalarScope(ParseLiteralScalar(map.Context, map.Indent + 2, val[1]), map.Indent + 2, map.Context, childTag));
            else if (val.StartsWith('{') && val.EndsWith('}'))
                return new(key, ParseFlow(map.Context, val, map.Indent + 2, childTag));
            else if (val.StartsWith('[') && val.EndsWith(']'))
                return new(key, SequenceScope.ParseFlow(map.Context, val, map.Indent + 2, childTag));
            else
                return new(key, new ScalarScope(val, map.Indent + 2, map.Context, childTag));
        }
    }
}
public struct BlockMappingPrefixedParse(MappingScope scope, string startKey, string initialValue) : IEnumerable<KeyValuePair<string, Scope>>
{
    public readonly IEnumerator<KeyValuePair<string, Scope>> GetEnumerator()
    {
        // If we were seeded with a key (from "- key:" or "- key: value")
        if (startKey != null)
        {
            if (!string.IsNullOrEmpty(initialValue))
            {
                string childTag = "";
                ReadOnlySpan<char> valSpan = initialValue.AsSpan();

                if (valSpan[0] == '!' && !valSpan.SequenceEqual("!!null".AsSpan()))
                {
                    int spaceIdx = valSpan.IndexOf(' ');
                    if (spaceIdx >= 0)
                    {
                        childTag = valSpan.Slice(0, spaceIdx).ToString();
                        valSpan = valSpan.Slice(spaceIdx + 1).Trim();
                    }
                    else
                    {
                        childTag = valSpan.ToString();
                        valSpan = ReadOnlySpan<char>.Empty;
                    }
                }

                yield return scope.StandardMappingResolve(scope, startKey, valSpan.ToString(), childTag);
            }
            else
            {
                // No inline value: "- key:" followed by nested mapping/sequence
                if (scope.Context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = Scope.CountIndent(lookahead);
                    var nextTrim = lookahead.AsSpan(nextIndent);

                    if (nextIndent > scope.Indent)
                    {
                        if (nextTrim[0] == '-')
                            yield return new(startKey, SequenceScope.Parse(scope.Context, scope.Indent + 2, ""));
                        else
                            yield return new(startKey, MappingScope.Parse(scope.Context, scope.Indent + 2, ""));
                    }
                    else
                    {
                        yield return new(startKey, new ScalarScope(string.Empty, scope.Indent + 2, scope.Context, ""));
                    }
                }
            }
        }

        // Continue parsing the rest of the mapping at this indent
        foreach (var m in MappingScope.Parse(scope.Context, scope.Indent, scope.Tag))
        {
            yield return new(m.Key, m.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public struct BlockFlowParse(MappingScope scope, string value) : IEnumerable<KeyValuePair<string, Scope>>
{
    public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator()
    {
        var inner = value.Substring(1, value.Length - 2).Trim();
        if (inner.Length == 0)
            yield break;

        foreach (var entry in Scope.SplitFlowItems(inner))
        {
            var kv = entry.Split(':', 2);
            if (kv.Length != 2)
                throw new InvalidOperationException($"Invalid inline mapping entry: '{entry}'");

            var key = kv[0].Trim();
            var val = kv[1].Trim();

            string childTag = "";
            if (val.StartsWith('!') && val != "!!null")
            {
                var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                childTag = segs[0];
                val = segs.Length > 1 ? segs[1].Trim() : "";
            }

            yield return scope.StandardMappingResolve(scope.Context, scope, key, val, childTag);

        }
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public struct BlockMappingParse(MappingScope scope) : IEnumerable<KeyValuePair<string, Scope>>
{
    public IEnumerator<KeyValuePair<string, Scope>> GetEnumerator()
    {

        while (scope.Context.Reader.Peek(out var next))
        {
            // Work with spans to avoid allocations
            ReadOnlySpan<char> line = next.AsSpan();

            int lineIndent = MappingScope.CountIndent(next);
            if (lineIndent < scope.Indent) break;
            scope.Indent = lineIndent;

            // Slice off leading spaces
            ReadOnlySpan<char> trimmed = line.Slice(lineIndent);
            if (trimmed.IsEmpty) { scope.Context.Reader.Move(); continue; }

            // Sequence indicator means mapping ends
            if (trimmed[0] == '-') break;

            // Standalone tag check
            if (trimmed[0] == '!' && !trimmed.SequenceEqual("!!null".AsSpan()))
                throw new InvalidOperationException($"Standalone tag inside mapping is invalid: '{next}'");

            // Find key/value separator
            int colonIdx = trimmed.IndexOf(':');
            if (colonIdx < 0)
                throw new InvalidOperationException($"Invalid mapping line: '{next}'");

            ReadOnlySpan<char> keySpan = trimmed.Slice(0, colonIdx).Trim();
            ReadOnlySpan<char> valSpan = colonIdx + 1 < trimmed.Length
                ? trimmed.Slice(colonIdx + 1).Trim()
                : ReadOnlySpan<char>.Empty;

            // Consume the line
            scope.Context.Reader.Move();

            string key = keySpan.ToString();
            ReadOnlySpan<char> val = valSpan;

            string childTag = "";

            // Inline tag handling
            MappingScope.ExtractTag(ref val, ref childTag);

            if (val.Length > 0)
            {
                yield return scope.StandardMappingResolve(scope.Context, scope, key, val.ToString(), childTag);
            }
            else
            {
                // Look ahead for nested structures
                if (scope.Context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = MappingScope.CountIndent(lookahead);
                    ReadOnlySpan<char> nextTrim = lookahead.AsSpan(nextIndent);

                    if (nextIndent > scope.Indent)
                    {
                        if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                            yield return new(key, SequenceScope.Parse(scope.Context, scope.Indent + 2, childTag));
                        else
                            yield return new(key, MappingScope.Parse(scope.Context, scope.Indent + 2, childTag));
                        continue;
                    }
                    else if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                    {
                        yield return new(key, SequenceScope.Parse(scope.Context, scope.Indent, childTag));
                        continue;
                    }
                }

                // Default: empty scalar
                yield return new(key, new ScalarScope(string.Empty, scope.Indent + 2, scope.Context, childTag));
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

