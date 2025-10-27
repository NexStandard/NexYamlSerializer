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
        // Continue with your existing loop
        while (context.Reader.Peek(out var next))
        {
            int lineIndent = CountIndent(next);
            if (lineIndent < map.Indent) break;
            map.Indent = lineIndent;

            string trimmed = next.Trim();

            if (trimmed.StartsWith('!') && trimmed != "!!null")
                throw new InvalidOperationException($"Standalone tag inside mapping is invalid: '{trimmed}'");


            if (next.Trim().StartsWith('-')) break;
            var parts = trimmed.Split(':', 2);
            if (parts.Length != 2) throw new InvalidOperationException($"Invalid mapping line: '{trimmed}'");
            context.Reader.Move(out var dump);

            var key = parts[0].Trim();
            var val = parts[1].Trim();


            string childTag = "";
            if (val.StartsWith('!') && val != "!!null")
            {
                var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                childTag = segs[0];
                val = segs.Length > 1 ? segs[1].Trim() : "";
            }

            if (val.Length > 0)
            {
                StandardMappingResolve(context,map, key, val, childTag);
            }
            else
            {
                context.Reader.Peek(out var dump4);
                int nextIndent = CountIndent(dump4);
                if (nextIndent > map.Indent)
                {
                    var nextTrim = dump4.TrimStart();
                    if (nextTrim.StartsWith('-'))
                        map.Add(key, YamlParser.Sequence.Parse(context, map.Indent + 2, childTag));
                    else
                        map.Add(key, Parse(context,map.Indent + 2, childTag));
                    continue;
                }
                else
                {
                    var nextTrim = dump4.TrimStart();
                    if (nextTrim.StartsWith('-'))
                        map.Add(key, YamlParser.Sequence.Parse(context,map.Indent, childTag));
                    continue;
                }
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
