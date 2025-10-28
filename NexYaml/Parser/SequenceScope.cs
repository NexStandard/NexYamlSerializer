using System.Collections;

namespace NexYaml.Parser
{
    public sealed class SequenceScope : Scope, IEnumerable<Scope>
    {
        private int _count;
        private bool nextGeneration = false;
        private string value;
        private bool flow;
        internal SequenceScope(int nonsense, int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
: base(tag, indent, context)
        {
            nextGeneration = true;
            _count = 0;
        }
        internal SequenceScope(int nonsense, string value, int indent, ScopeContext context, string tag = "", int initialCapacity = 10)
: base(tag, indent, context)
        {
            nextGeneration = true;
            flow = true;
            this.value = value;
            _count = 0;
        }
        public static SequenceScope Parse(ScopeContext context, int indent, string tag)
        {
            var map = new SequenceScope(1, indent, context, tag);
            return map;
        }
        public static SequenceScope ParseFlow(ScopeContext context, string value, int indent, string tag)
        {
            return new SequenceScope(1, value, indent, context, tag);
        }
        public override ScopeKind Kind => ScopeKind.Sequence;

        public IEnumerator<Scope> GetEnumerator()
        {
                if (flow)
                {
                    foreach(var ele in ParseSequenceFlow(Context, value, Indent, Tag))
                    {
                        yield return ele;
                    }
                }
                else
                {
                    foreach(var ele in ParseSequence(Context, Indent, Tag))
                    {
                        yield return ele;
                    }
                }
        }
        private IEnumerable<Scope> ParseSequence(ScopeContext context, int indent, string tag)
        {
            while (context.Reader.Peek(out var next))
            {
                int lineIndent = CountIndent(next);
                if (lineIndent < indent) yield break;
                if (lineIndent != indent) yield break;

                ReadOnlySpan<char> line = next.AsSpan(lineIndent);
                if (line.Length == 0 || line[0] != '-') yield break;

                // consume this line
                context.Reader.Move();

                // skip the leading '-' and any following spaces
                int i = 1;
                while (i < line.Length && line[i] == ' ')
                    i++;

                var itemSpan = line.Slice(i).Trim();
                string childTag = "";

                // tag detection
                ExtractTag(ref itemSpan, ref childTag);

                if (itemSpan.Length > 0)
                {
                    // quoted scalar
                    if (IsQuoted(itemSpan))
                    {
                        yield return new ScalarScope(Unquote(itemSpan.ToString()), indent + 2, context, childTag);
                    }
                    // literal block scalar
                    else if (itemSpan[0] == '|')
                    {
                        yield return new ScalarScope(ParseLiteralScalar(Context, indent + 1, itemSpan[1]), indent + 2, context, childTag);
                    }
                    // flow mapping
                    else if (itemSpan[0] == '{' && itemSpan[^1] == '}')
                    {
                        yield return MappingScope.ParseFlow(context, itemSpan.ToString(), indent + 2, childTag);
                    }
                    // flow sequence
                    else if (itemSpan[0] == '[' && itemSpan[^1] == ']')
                    {
                        yield return ParseFlow(context, itemSpan.ToString(), indent + 2, childTag);
                    }
                    // inline mapping (key: value)
                    else
                    {
                        int colonIdx = itemSpan.IndexOf(':');
                        if (colonIdx >= 0)
                        {
                            var keySpan = itemSpan.Slice(0, colonIdx).Trim();
                            var valSpan = colonIdx + 1 < itemSpan.Length
                                ? itemSpan.Slice(colonIdx + 1).Trim()
                                : ReadOnlySpan<char>.Empty;

                            yield return new MappingScope(1,  valSpan.ToString(), keySpan.ToString(), indent + 2, context, childTag);
                        }
                        else
                        {
                            yield return new ScalarScope(itemSpan.ToString(), indent + 2, context, childTag);
                        }
                    }
                }
                else
                {
                    // empty item, look ahead for nested structure
                    if (context.Reader.Peek(out var la))
                    {
                        int nextIndent = CountIndent(la);
                        var nextTrim = la.AsSpan(nextIndent);

                        if (nextIndent > indent)
                        {
                            if (nextTrim.Length > 0 && nextTrim[0] == '-')
                                yield return Parse(context, indent + 2, childTag);
                            else
                                yield return MappingScope.Parse(context, indent + 2, childTag);
                            continue;
                        }
                    }

                    yield return new ScalarScope(string.Empty, indent + 2, context, childTag);
                }
            }
        }
        public IEnumerable<Scope> ParseSequenceFlow(ScopeContext context, string value, int indent, string tag)
        {
            var inner = value.Substring(1, value.Length - 2).Trim();
            if (inner.Length == 0)
                yield break;
            string bufferedTag = "";
            foreach (var raw in SplitFlowItems(inner))
            {
                string childTag = "";
                string item = raw;
                if (bufferedTag == "")
                {
                    if (item.StartsWith('!') && item != "!!null")
                    {
                        var segs = item.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                        if (segs.Length == 1)
                        {
                            bufferedTag = segs[0];
                            continue;
                        }
                        else
                        {
                            childTag = segs[0];
                            item = segs.Length > 1 ? segs[1].Trim() : "";

                        }
                    }
                }
                else
                {
                    childTag = bufferedTag;
                    bufferedTag = "";
                }

                if (IsQuoted(item))
                    yield return new ScalarScope(Unquote(item), indent + 2, context, childTag);
                else if (item.StartsWith('|'))
                    yield return new ScalarScope(ParseLiteralScalar(context, indent + 2, item[1]), indent + 2, context, childTag);
                else if (item.StartsWith('{') && item.EndsWith('}'))
                    yield return MappingScope.ParseFlow(context, item, indent + 2, childTag);
                else if (item.StartsWith('[') && item.EndsWith(']'))
                    yield return ParseFlow(context, item, indent + 2, childTag);
                else
                    yield return new ScalarScope(item, indent + 2, context, childTag);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
