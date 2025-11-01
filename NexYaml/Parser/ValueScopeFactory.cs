﻿using NexYaml.Core;
using NexYaml.Parser;

class ValueScopeFactory
{
    public Scope Parse(ScopeContext context, string val, int indent, string tag)
    {
        var valSpan = val.AsSpan();
        if (valSpan.StartsWith('!') && val != YamlCodes.Null)
        {
            var spaceIdx = valSpan.IndexOf(' ');
            var childTag = valSpan[..spaceIdx].ToString();

            // Skip spaces
            do
            {
                spaceIdx++;
            } while (spaceIdx < valSpan.Length && valSpan[spaceIdx] == ' ');

            // Skip first character
            // Replicated the base logic, but not sure why we would skip the first character of what's after the space though -Eideren
            string rest = spaceIdx + 1 < val.Length ? valSpan[(spaceIdx + 1)..].ToString() : string.Empty;
            return Parse(context, rest, indent, childTag);
        }

        if (Scope.TryGetQuotedText(valSpan, out var unquotedSpan))
            return new ScalarScope(unquotedSpan.ToString(), indent, context, tag);
        if (valSpan.StartsWith('|'))
            return new ScalarScope(Scope.ParseLiteralScalar(context, indent, valSpan[1]), indent, context, tag);
        if (valSpan.StartsWith('{') && valSpan.EndsWith('}'))
            return MappingScope.ParseFlow(context, val, indent, tag);
        if (valSpan.StartsWith('[') && valSpan.EndsWith(']'))
            return SequenceScope.ParseFlow(context, val, indent, tag);

        return new ScalarScope(val, indent, context, tag);
    }

    public Scope Parse(ScopeContext context, int indent, string tag)
    {
        if (context.Reader.Move(out var val))
        {
            return Parse(context, val.Trim(), indent, tag);
        }
        throw new EndOfStreamException();
    }

    public Scope ParseFlow(ScopeContext context, string value, int indent, string tag) => throw new NotSupportedException();
}
