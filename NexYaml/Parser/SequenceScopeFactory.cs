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
            string trimmed = next.Trim();
            if (!trimmed.StartsWith('-')) break;
            context.Reader.Move();

            // Skip the leading '-' and any following spaces
            int i = 1;
            while (i < trimmed.Length && trimmed[i] == ' ')
                i++;

            var item = trimmed.Substring(i);

            string childTag = "";

            if (item.StartsWith('!') && item != "!!null")
            {
                var segs = item.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                childTag = segs[0];
                item = segs.Length > 1 ? segs[1].Trim() : "";
            }

            if (item.Length > 0)
            {
                if (IsQuoted(item))
                    seq.Add(new ScalarScope(Unquote(item), indent + 2, context, childTag));
                else if (item.StartsWith('|'))
                    seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, context, childTag));
                else if (item.StartsWith('{') && item.EndsWith('}'))
                    seq.Add(YamlParser.Mapping.ParseFlow(context,item, indent + 2, childTag));
                else if (item.StartsWith('[') && item.EndsWith(']'))
                    seq.Add(ParseFlow(context,item, indent + 2, childTag));
                else if (item.Contains(':'))
                {
                    var parts = item.Split(':', 2);
                    var key = parts[0].Trim();
                    var val = parts.Length > 1 ? parts[1].Trim() : "";

                    // Delegate into ParseMapping with seed key/value
                    seq.Add(ParseMapping(context,indent + 2, childTag, key, val));
                }
                else
                    seq.Add(new ScalarScope(item, indent + 2, context, childTag));
            }
            else
            {

                context.Reader.Peek(out var dump);
                int nextIndent = CountIndent(dump);
                if (nextIndent > indent)
                {
                    var nextTrim = dump.TrimStart();
                    if (nextTrim.StartsWith('-'))
                        seq.Add(Parse(context, indent + 2, childTag));
                    else
                    {
                        seq.Add(YamlParser.Mapping.Parse(context, indent + 2, childTag));
                    }
                    continue;
                }
            }
        }
        return seq;
    }
    private MappingScope ParseMapping(ScopeContext context,int indent, string tag, string? key = null, string? initialValue = null)
    {
        var map = new MappingScope(indent, context, tag);

        // If we were seeded with a key (from "- key:" or "- key: value")
        if (key != null)
        {
            if (!string.IsNullOrEmpty(initialValue))
            {
                string childTag = "";
                string val = initialValue;

                if (val.StartsWith('!') && val != "!!null")
                {
                    var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    val = segs.Length > 1 ? segs[1].Trim() : "";
                }

                StandardMappingResolve(context,map, key, val, childTag);
            }
            else
            {

                context.Reader.Peek(out var dump);
                int nextIndent = CountIndent(dump);
                if (nextIndent > indent)
                {
                    var nextTrim = dump.TrimStart();
                    if (nextTrim.StartsWith('-'))
                        map.Add(key, Parse(context,indent + 2, ""));
                    else
                        map.Add(key, YamlParser.Mapping.Parse(context,indent + 2, ""));
                }
                else
                {
                    map.Add(key, new ScalarScope(string.Empty, indent + 2, context, ""));
                }

            }
        }
        var temp = YamlParser.Mapping.Parse(context, map.Indent, map.Tag);
        foreach(var m in temp)
        {
            map.Add(m.Key,m.Value);
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
                seq.Add(new ScalarScope(Unquote(item), indent + 2,context, childTag));
            else if (item.StartsWith('|'))
                seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, context, childTag));
            else if (item.StartsWith('{') && item.EndsWith('}'))
                seq.Add(YamlParser.Mapping.ParseFlow(context, item, indent + 2, childTag));
            else if (item.StartsWith('[') && item.EndsWith(']'))
                seq.Add(ParseFlow(context, item, indent + 2, childTag));
            else
                seq.Add(new ScalarScope(item, indent + 2, context, childTag));
        }

        return seq;
    }
    private void StandardMappingResolve(ScopeContext context, MappingScope map, string key, string val, string childTag)
    {
        if (IsQuoted(val))
            map.Add(key, new ScalarScope(Unquote(val), map.Indent + 2, context, childTag));
        else if (val.StartsWith('|'))
            map.Add(key, new ScalarScope(ParseLiteralScalar(map.Indent + 2), map.Indent + 2, context, childTag));
        else if (val.StartsWith('{') && val.EndsWith("}"))
            map.Add(key, ParseFlow(context, val, map.Indent + 2, childTag));
        else if (val.StartsWith('[') && val.EndsWith("]"))
            map.Add(key, YamlParser.Sequence.ParseFlow(context, val, map.Indent + 2, childTag));
        else
            map.Add(key, new ScalarScope(val, map.Indent + 2, context, childTag));
    }
}
