using NexYaml;
using NexYaml.Parser;

class ValueScopeFactory : ScopeFactory<Scope>
{
    public override Scope Parse(ScopeContext context, int indent, string tag)
    {
        if (context.Reader.Move(out var val))
        {
            return Parse(context, val.Trim(), indent, tag);
        }
        throw new EndOfStreamException();
    }
    public Scope Parse(ScopeContext context, string val, int indent, string tag)
    {
        if (val.StartsWith('!') && val != "!!null")
        {
            var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            string childTag = segs[0];
            string rest = segs.Length > 1 ? segs[1].Trim() : "";
            return Parse(context, rest, indent, childTag);
        }

        if (IsQuoted(val))
            return new ScalarScope(Unquote(val), indent, context, tag);
        if (val.StartsWith('|'))
            return new ScalarScope(ParseLiteralScalar(context,indent, val[1]), indent, context, tag);
        if (val.StartsWith('{') && val.EndsWith('}'))
            return MappingScope.ParseFlow(context, val, indent, tag);
        if (val.StartsWith('[') && val.EndsWith(']'))
            return SequenceScope.ParseFlow(context, val, indent, tag);

        return new ScalarScope(val, indent, context, tag);
    }

    public override Scope ParseFlow(ScopeContext context, string value, int indent, string tag) => throw new NotSupportedException();
}
