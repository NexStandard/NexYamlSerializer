using NexYaml;
using NexYaml.Parser;

class SequenceScopeFactory : ScopeFactory<SequenceScope>
{
    public override SequenceScope Parse(ScopeContext context, int indent, string tag)
    {
        var seq = new SequenceScope(indent, context, tag);

        while (context.Reader.Peek(out var next))
        {
            int lineIndent = CountIndent(next);
            if (lineIndent < indent) break;
            if (lineIndent != indent) break;

            ReadOnlySpan<char> line = next.AsSpan(lineIndent);
            if (line.Length == 0 || line[0] != '-') break;

            // consume this line
            context.Reader.Move();

            // skip the leading '-' and any following spaces
            int i = 1;
            while (i < line.Length && line[i] == ' ')
                i++;

            var itemSpan = line.Slice(i).Trim();
            string childTag = "";

            // tag detection
            if (itemSpan.Length > 0 && itemSpan[0] == '!' && !itemSpan.SequenceEqual("!!null".AsSpan()))
            {
                int spaceIdx = itemSpan.IndexOf(' ');
                if (spaceIdx >= 0)
                {
                    childTag = itemSpan.Slice(0, spaceIdx).ToString();
                    itemSpan = itemSpan.Slice(spaceIdx + 1).Trim();
                }
                else
                {
                    childTag = itemSpan.ToString();
                    itemSpan = ReadOnlySpan<char>.Empty;
                }
            }

            if (itemSpan.Length > 0)
            {
                // quoted scalar
                if (IsQuoted(itemSpan))
                {
                    seq.Add(new ScalarScope(Unquote(itemSpan.ToString()), indent + 2, context, childTag));
                }
                // literal block scalar
                else if (itemSpan[0] == '|')
                {
                    seq.Add(new ScalarScope(ParseLiteralScalar(seq.Context,indent + 1, itemSpan[1]), indent + 2, context, childTag));
                }
                // flow mapping
                else if (itemSpan[0] == '{' && itemSpan[^1] == '}')
                {
                    seq.Add(YamlParser.Mapping.ParseFlow(context, itemSpan.ToString(), indent + 2, childTag));
                }
                // flow sequence
                else if (itemSpan[0] == '[' && itemSpan[^1] == ']')
                {
                    seq.Add(ParseFlow(context, itemSpan.ToString(), indent + 2, childTag));
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

                        seq.Add(ParseMapping(context,
                                             indent + 2,
                                             childTag,
                                             keySpan.ToString(),
                                             valSpan.ToString()));
                    }
                    else
                    {
                        seq.Add(new ScalarScope(itemSpan.ToString(), indent + 2, context, childTag));
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
                            seq.Add(Parse(context, indent + 2, childTag));
                        else
                            seq.Add(YamlParser.Mapping.Parse(context, indent + 2, childTag));
                        continue;
                    }
                }

                seq.Add(new ScalarScope(string.Empty, indent + 2, context, childTag));
            }
        }

        return seq;
    }

    private MappingScope ParseMapping(ScopeContext context, int indent, string tag, string? key = null, string? initialValue = null)
    {
        var map = new MappingScope(indent, context, tag);

        // If we were seeded with a key (from "- key:" or "- key: value")
        if (key != null)
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

                StandardMappingResolve(map, key, valSpan.ToString(), childTag);
            }
            else
            {
                // No inline value: "- key:" followed by nested mapping/sequence
                if (context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = CountIndent(lookahead);
                    var nextTrim = lookahead.AsSpan(nextIndent);

                    if (nextIndent > indent)
                    {
                        if (nextTrim[0] == '-')
                            map.Add(key, Parse(context, indent + 2, ""));
                        else
                            map.Add(key, YamlParser.Mapping.Parse(context, indent + 2, ""));
                    }
                    else
                    {
                        map.Add(key, new ScalarScope(string.Empty, indent + 2, context, ""));
                    }
                }
            }
        }

        // Continue parsing the rest of the mapping at this indent
        var temp = YamlParser.Mapping.Parse(context, map.Indent, map.Tag);
        foreach (var m in temp)
        {
            map.Add(m.Key, m.Value);
        }

        return map;
    }
    public override SequenceScope ParseFlow(ScopeContext context, string value, int indent, string tag)
    {
        var seq = new SequenceScope(indent, context, tag);
        var inner = value.Substring(1, value.Length - 2).Trim();
        if (inner.Length == 0) return seq;
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
                seq.Add(new ScalarScope(Unquote(item), indent + 2, context, childTag));
            else if (item.StartsWith('|'))
                seq.Add(new ScalarScope(ParseLiteralScalar(context,indent + 2, item[1]), indent + 2, context, childTag));
            else if (item.StartsWith('{') && item.EndsWith('}'))
                seq.Add(YamlParser.Mapping.ParseFlow(context, item, indent + 2, childTag));
            else if (item.StartsWith('[') && item.EndsWith(']'))
                seq.Add(ParseFlow(context, item, indent + 2, childTag));
            else
                seq.Add(new ScalarScope(item, indent + 2, context, childTag));
        }

        return seq;
    }
    private void StandardMappingResolve(MappingScope map, string key, string val, string childTag)
    {
        if (IsQuoted(val))
            map.Add(key, new ScalarScope(Unquote(val), map.Indent + 2, map.Context, childTag));
        else if (val.StartsWith('|'))
            map.Add(key, new ScalarScope(ParseLiteralScalar(map.Context,map.Indent + 2, val[1]), map.Indent + 2, map.Context, childTag));
        else if (val.StartsWith('{') && val.EndsWith('}'))
            map.Add(key, ParseFlow(map.Context, val, map.Indent + 2, childTag));
        else if (val.StartsWith('[') && val.EndsWith(']'))
            map.Add(key, YamlParser.Sequence.ParseFlow(map.Context, val, map.Indent + 2, childTag));
        else
            map.Add(key, new ScalarScope(val, map.Indent + 2, map.Context, childTag));
    }
}
