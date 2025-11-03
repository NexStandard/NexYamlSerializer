using System.Collections;
using NexYaml.Core;

namespace NexYaml.Parser
{
    public sealed class SequenceScope : Scope, IEnumerable<Scope>
    {
        private string? flowValue;

        public override ScopeKind Kind => ScopeKind.Sequence;

        internal SequenceScope(int indent, ScopeContext context, string tag) : base(tag, indent, context)
        {
            flowValue = null;
        }

        internal SequenceScope(string flowValueParam, int indent, ScopeContext context, string tag) : base(tag, indent, context)
        {
            flowValue = flowValueParam;
        }

        public static SequenceScope Parse(ScopeContext context, int indent, string tag)
        {
            return new SequenceScope(indent, context, tag);
        }

        public static SequenceScope ParseFlow(ScopeContext context, string value, int indent, string tag)
        {
            return new SequenceScope(value, indent, context, tag);
        }

        public IEnumerator<Scope> GetEnumerator()
        {
            if (flowValue is not null)
            {
                foreach(var ele in ParseSequenceFlow(Context, flowValue, Indent))
                {
                    yield return ele;
                }
            }
            else
            {
                foreach(var ele in ParseSequence(Context, Indent))
                {
                    yield return ele;
                }
            }
        }

        private IEnumerable<Scope> ParseSequence(ScopeContext context, int indent)
        {
            while (context.Reader.Peek(out var next))
            {
                int lineIndent = CountIndent(next);
                if (lineIndent != indent)
                    yield break;

                ReadOnlySpan<char> line = next.AsSpan(lineIndent);
                if (line.Length == 0 || line[0] != '-')
                    yield break;

                // consume this line
                context.Reader.Move();

                // skip the leading '-' and any following spaces
                var itemSpan = line[1..].Trim();

                // tag detection
                ExtractTag(ref itemSpan, out var childTag);

                if (itemSpan.Length > 0)
                {
                    switch (itemSpan[0])
                    {
                        // quoted scalar
                        case '\"' when TryGetQuotedText(itemSpan, out var unquotedItemSpan):
                            yield return new ScalarScope(unquotedItemSpan.ToString(), indent + 2, context, childTag.ToString());
                            continue;
                        // literal block scalar
                        case '|':
                            yield return new ScalarScope(ParseLiteralScalar(Context, indent + 1, itemSpan[1]), indent + 2, context, childTag.ToString());
                            continue;
                        // flow mapping
                        case '{' when itemSpan[^1] == '}':
                            yield return MappingScope.ParseFlow(context, itemSpan.ToString(), indent + 2, childTag.ToString());
                            continue;
                        // flow sequence
                        case '[' when itemSpan[^1] == ']':
                            yield return ParseFlow(context, itemSpan.ToString(), indent + 2, childTag.ToString());
                            continue;
                        // inline mapping (key: value)
                        default:
                            int colonIdx = itemSpan.IndexOf(':');
                            if (colonIdx >= 0)
                            {
                                var keySpan = itemSpan[..colonIdx].Trim();
                                var valSpan = itemSpan[(colonIdx + 1)..].Trim();

                                yield return new MappingScope(valSpan.ToString(), keySpan.ToString(), indent + 2, context, childTag.ToString());
                            }
                            else
                            {
                                yield return new ScalarScope(itemSpan.ToString(), indent + 2, context, childTag.ToString());
                            }

                            continue;
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
                                yield return Parse(context, indent + 2, childTag.ToString());
                            else
                                yield return MappingScope.Parse(context, indent + 2, childTag.ToString());
                            continue;
                        }
                    }

                    yield return new ScalarScope(string.Empty, indent + 2, context, childTag.ToString());
                }
            }
        }

        public static IEnumerable<Scope> ParseSequenceFlow(ScopeContext context, string value, int indent)
        {
            var inner = value.Substring(1, value.Length - 2).Trim();
            if (inner.Length == 0)
                yield break;

            string bufferedTag = string.Empty;
            foreach (var raw in SplitFlowItems(inner))
            {
                string childTag = string.Empty;
                string item = raw;
                if (bufferedTag == string.Empty)
                {
                    if (item.StartsWith('!') && item != YamlCodes.Null)
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
                            item = segs.Length > 1 ? segs[1].Trim() : string.Empty;

                        }
                    }
                }
                else
                {
                    childTag = bufferedTag;
                    bufferedTag = string.Empty;
                }

                if (TryGetQuotedText(item, out var unquotedItem))
                    yield return new ScalarScope(unquotedItem.ToString(), indent + 2, context, childTag);
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
