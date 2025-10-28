using NexYaml;
using NexYaml.Parser;

class MappingScopeFactory : ScopeFactory<MappingScope>
{
    public override MappingScope Parse(ScopeContext context, int indent, string tag)
    {
        var map = new MappingScope(indent, context, tag);
        ParseMappingLoop(map,context);
        return map;
    }

    public override MappingScope ParseFlow(ScopeContext context,string value, int indent, string tag)
    {
        var map = new MappingScope(indent, context, tag);
        var inner = value.Substring(1, value.Length - 2).Trim();
        if (inner.Length == 0) return map;

        foreach (var entry in SplitFlowItems(inner))
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

            StandardMappingResolve(context,map, key, val, childTag);
        }
        return map;
    }
    private void ParseMappingLoop(MappingScope map, ScopeContext context)
    {
        while (context.Reader.Peek(out var next))
        {
            // Work with spans to avoid allocations
            ReadOnlySpan<char> line = next.AsSpan();

            int lineIndent = CountIndent(next);
            if (lineIndent < map.Indent) break;
            map.Indent = lineIndent;

            // Slice off leading spaces
            ReadOnlySpan<char> trimmed = line.Slice(lineIndent);
            if (trimmed.IsEmpty) { context.Reader.Move(); continue; }

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
            context.Reader.Move();

            string key = keySpan.ToString();
            string val = valSpan.ToString();

            string childTag = "";

            // Inline tag handling
            if (!valSpan.IsEmpty && valSpan[0] == '!' && !valSpan.SequenceEqual("!!null".AsSpan()))
            {
                int spaceIdx = valSpan.IndexOf(' ');
                if (spaceIdx >= 0)
                {
                    childTag = valSpan.Slice(0, spaceIdx).ToString();
                    val = valSpan.Slice(spaceIdx + 1).Trim().ToString();
                }
                else
                {
                    childTag = val;
                    val = string.Empty;
                }
            }

            if (val.Length > 0)
            {
                StandardMappingResolve(context, map, key, val, childTag);
            }
            else
            {
                // Look ahead for nested structures
                if (context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = CountIndent(lookahead);
                    ReadOnlySpan<char> nextTrim = lookahead.AsSpan(nextIndent);

                    if (nextIndent > map.Indent)
                    {
                        if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                            map.Add(key, YamlParser.Sequence.Parse(context, map.Indent + 2, childTag));
                        else
                            map.Add(key, Parse(context, map.Indent + 2, childTag));
                        continue;
                    }
                    else if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                    {
                        map.Add(key, YamlParser.Sequence.Parse(context, map.Indent, childTag));
                        continue;
                    }
                }

                // Default: empty scalar
                map.Add(key, new ScalarScope(string.Empty, map.Indent + 2, context, childTag));
            }
        }
    }
    private void StandardMappingResolve(ScopeContext context,MappingScope map, string key, string val, string childTag)
    {
        if (IsQuoted(val))
            map.Add(key, new ScalarScope(Unquote(val), map.Indent + 2, context, childTag));
        else if (val.StartsWith('|'))
            map.Add(key, new ScalarScope(ParseLiteralScalar(map.Indent + 2), map.Indent + 2, context, childTag));
        else if (val.StartsWith('{') && val.EndsWith("}"))
            map.Add(key, ParseFlow(context, val, map.Indent + 2, childTag));
        else if (val.StartsWith('[') && val.EndsWith("]"))
            map.Add(key, YamlParser.Sequence.ParseFlow(context,val, map.Indent + 2, childTag));
        else
            map.Add(key, new ScalarScope(val, map.Indent + 2, context, childTag));
    }

}
