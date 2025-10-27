using System.Diagnostics.CodeAnalysis;
using System.Text;
using NexYaml.Serialization;

namespace NexYaml.Parser
{
    public sealed class YamlParser : IDisposable
    {
        private readonly StreamReader _reader;
        private readonly IYamlSerializerResolver _resolver;
        private IdentifiableResolver IdentifiableResolver { get; } = new();
        private YamlReader xreader;
        public YamlParser(string text, IYamlSerializerResolver resolver)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var ms = new MemoryStream(bytes);
            _reader = new StreamReader(ms, Encoding.UTF8, leaveOpen: false);
            _resolver = resolver;
            xreader = new YamlReader()
            {
                Reader = _reader
            };
        }

        public YamlParser(Stream stream, IYamlSerializerResolver resolver, Encoding? encoding = null)
        {
            _reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen: true);
            _resolver = resolver;
            xreader = new YamlReader()
            {
                Reader = _reader
            };
        }
        public IEnumerable<Scope> Parse()
        {
            while (xreader.Peek(out var currentLine))
            {
                if (string.IsNullOrWhiteSpace(currentLine)) continue;

                int indent = CountIndent(currentLine);
                string trimmed = currentLine.Trim();

                // Tagged root
                if (trimmed.StartsWith('!') && trimmed != "!!null")
                {
                    xreader.Move(out var scope);
                    int spaceIndex = trimmed.IndexOf(' ');
                    string tag = spaceIndex > 0 ? trimmed.Substring(0, spaceIndex) : trimmed;
                    string inline = spaceIndex > 0 ? trimmed.Substring(spaceIndex + 1).Trim() : "";

                    if (inline.Length > 0)
                    {
                        yield return ParseValue(inline, indent, tag);
                        continue;
                    }

                    if (!xreader.Peek(out var nextLine))
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not followed by a value");


                    int nextIndent = CountIndent(nextLine);
                    if (nextIndent != indent)
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not aligned with following value");

                    if (nextLine.TrimStart().StartsWith('-'))
                    {
                        yield return ParseSequence(indent, tag);
                    }
                    else if (nextLine.Contains(':'))
                    {
                        yield return ParseMapping(indent, tag);
                    }
                    else
                    {
                        xreader.Move(out var s);
                        yield return ParseValue(nextLine.Trim(), indent, tag);
                    }

                    continue;
                }

                // Sequence root
                if (trimmed.StartsWith('-'))
                {
                    yield return ParseSequence(indent, "");
                }
                // Mapping root
                else if (trimmed.Contains(':'))
                {
                    yield return ParseMapping(indent, "");
                }
                // Scalar root
                else
                {
                    xreader.Move(out var scope);
                    yield return ParseValue(trimmed, indent, "");
                }
            }
        }

        private Scope ParseValue(string val, int indent, string tag)
        {
            if (val.StartsWith('!') && val != "!!null")
            {
                var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                string childTag = segs[0];
                string rest = segs.Length > 1 ? segs[1].Trim() : "";
                return ParseValue(rest, indent, childTag);
            }

            if (IsQuoted(val))
                return new ScalarScope(Unquote(val), indent, _resolver, IdentifiableResolver, tag);
            if (val.StartsWith('|'))
                return new ScalarScope(ParseLiteralScalar(indent), indent, _resolver, IdentifiableResolver, tag);
            if (val.StartsWith('{') && val.EndsWith('}'))
                return ParseFlowMapping(val, indent, tag);
            if (val.StartsWith('[') && val.EndsWith(']'))
                return ParseFlowSequence(val, indent, tag);

            return new ScalarScope(val, indent, _resolver, IdentifiableResolver, tag);
        }

        private MappingScope ParseMapping(int indent, string tag)
        {
            var map = new MappingScope(indent, _resolver, IdentifiableResolver, tag);
            ParseMappingLoop(map);
            return map;
        }

        private MappingScope ParseMapping(int indent, string tag, string? key = null, string? initialValue = null)
        {
            var map = new MappingScope(indent, _resolver, IdentifiableResolver, tag);

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

                    StandardMappingResolve(map, key, val, childTag);
                }
                else
                {

                    xreader.Peek(out var dump);
                    int nextIndent = CountIndent(dump);
                    if (nextIndent > indent)
                    {
                        var nextTrim = dump.TrimStart();
                        if (nextTrim.StartsWith('-'))
                            map.Add(key, ParseSequence(indent + 2, ""));
                        else
                            map.Add(key, ParseMapping(indent + 2, ""));
                    }
                    else
                    {
                        map.Add(key, new ScalarScope(string.Empty, indent + 2, _resolver, IdentifiableResolver, ""));
                    }

                }
            }

            ParseMappingLoop(map);

            return map;
        }
        private void StandardMappingResolve(MappingScope map, string key, string val, string childTag)
        {
            if (IsQuoted(val))
                map.Add(key, new ScalarScope(Unquote(val), map.Indent + 2, _resolver, IdentifiableResolver, childTag));
            else if (val.StartsWith('|'))
                map.Add(key, new ScalarScope(ParseLiteralScalar(map.Indent + 2), map.Indent + 2, _resolver, IdentifiableResolver, childTag));
            else if (val.StartsWith('{') && val.EndsWith("}"))
                map.Add(key, ParseFlowMapping(val, map.Indent + 2, childTag));
            else if (val.StartsWith('[') && val.EndsWith("]"))
                map.Add(key, ParseFlowSequence(val, map.Indent + 2, childTag));
            else
                map.Add(key, new ScalarScope(val, map.Indent + 2, _resolver, IdentifiableResolver, childTag));
        }

        private MappingScope ParseFlowMapping(string text, int indent, string tag)
        {
            var map = new MappingScope(indent, _resolver, IdentifiableResolver, tag);
            var inner = text.Substring(1, text.Length - 2).Trim();
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

                StandardMappingResolve(map, key, val, childTag);
            }
            return map;
        }
        private SequenceScope ParseSequence(int indent, string tag)
        {
            var seq = new SequenceScope(indent, _resolver, IdentifiableResolver, tag);


            while (xreader.Peek(out var next))
            {
                int lineIndent = CountIndent(next);
                if (lineIndent < indent) break;
                if (lineIndent != indent) break;
                string trimmed = next.Trim();
                if (!trimmed.StartsWith('-')) break;
                xreader.Move();

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
                        seq.Add(new ScalarScope(Unquote(item), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (item.StartsWith('|'))
                        seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (item.StartsWith('{') && item.EndsWith('}'))
                        seq.Add(ParseFlowMapping(item, indent + 2, childTag));
                    else if (item.StartsWith('[') && item.EndsWith(']'))
                        seq.Add(ParseFlowSequence(item, indent + 2, childTag));
                    else if (item.Contains(':'))
                    {
                        var parts = item.Split(':', 2);
                        var key = parts[0].Trim();
                        var val = parts.Length > 1 ? parts[1].Trim() : "";

                        // Delegate into ParseMapping with seed key/value
                        seq.Add(ParseMapping(indent + 2, childTag, key, val));
                    }
                    else
                        seq.Add(new ScalarScope(item, indent + 2, _resolver, IdentifiableResolver, childTag));
                }
                else
                {

                    xreader.Peek(out var dump);
                    int nextIndent = CountIndent(dump);
                    if (nextIndent > indent)
                    {
                        var nextTrim = dump.TrimStart();
                        if (nextTrim.StartsWith('-'))
                            seq.Add(ParseSequence(indent + 2, childTag));
                        else
                        {
                            seq.Add(ParseMapping(indent + 2, childTag));
                        }
                        continue;
                    }
                }
            }
            return seq;
        }

        private void ParseMappingLoop(MappingScope map)
        {
            // Continue with your existing loop
            while (xreader.Peek(out var next))
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
                xreader.Move(out var dump);

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
                    StandardMappingResolve(map, key, val, childTag);
                }
                else
                {
                    xreader.Peek(out var dump4);
                    int nextIndent = CountIndent(dump4);
                    if (nextIndent > map.Indent)
                    {
                        var nextTrim = dump4.TrimStart();
                        if (nextTrim.StartsWith('-'))
                            map.Add(key, ParseSequence(map.Indent + 2, childTag));
                        else
                            map.Add(key, ParseMapping(map.Indent + 2, childTag));
                        continue;
                    }
                    else
                    {
                        var nextTrim = dump4.TrimStart();
                        if (nextTrim.StartsWith('-'))
                            map.Add(key, ParseSequence(map.Indent, childTag));
                        continue;
                    }
                }

            }
        }
        private SequenceScope ParseFlowSequence(string text, int indent, string tag)
        {
            var seq = new SequenceScope(indent, _resolver, IdentifiableResolver, tag);
            var inner = text.Substring(1, text.Length - 2).Trim();
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
                    seq.Add(new ScalarScope(Unquote(item), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (item.StartsWith('|'))
                    seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (item.StartsWith('{') && item.EndsWith('}'))
                    seq.Add(ParseFlowMapping(item, indent + 2, childTag));
                else if (item.StartsWith('[') && item.EndsWith(']'))
                    seq.Add(ParseFlowSequence(item, indent + 2, childTag));
                else
                    seq.Add(new ScalarScope(item, indent + 2, _resolver, IdentifiableResolver, childTag));
            }

            return seq;
        }

        private string ParseLiteralScalar(int indent)
        {
            throw new NotImplementedException();
        }

        private static bool IsQuoted(string s)
        {
            return s.Length >= 2 &&
                   ((s.StartsWith('\"') && s.EndsWith('\"')) ||
                    (s.StartsWith('\'') && s.EndsWith('\'')));
        }

        private static string Unquote(string s)
        {
            return IsQuoted(s) ? s.Substring(1, s.Length - 2) : s;
        }

        private static int CountIndent(string line)
        {
            var span = line.AsSpan();
            int i = 0;
            while (i < span.Length && span[i] == ' ')
                i++;
            return i;
        }

        private static IEnumerable<string> SplitFlowItems(string input)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            int depth = 0;
            bool inQuotes = false;
            bool inTag = false;
            char quoteChar = '\0';

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (inQuotes)
                {
                    sb.Append(c);
                    if (c == quoteChar) inQuotes = false;
                    continue;
                }

                if (c == '"' || c == '\'')
                {
                    inQuotes = true;
                    quoteChar = c;
                    sb.Append(c);
                    continue;
                }
                if (c == '!' && sb.ToString().Trim().Length == 0)
                {
                    inTag = true;
                }
                if ((c == ' ' || c == '[' || c == '{') && inTag)
                {
                    inTag = false;
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                    continue;
                }
                if (c == '[' || c == '{') depth++;
                if (c == ']' || c == '}') depth--;

                if (c == ',' && depth == 0 && !inTag)
                {
                    result.Add(sb.ToString().Trim());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
                result.Add(sb.ToString().Trim());

            return result;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
