using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.XParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NexYaml.Serialization;

    public sealed class YamlParser : IDisposable
    {
        private readonly StreamReader _reader;
        private string? _currentLine;
        private bool _eof;
        private IYamlSerializerResolver _resolver;
        private IdentifiableResolver IdentifiableResolver { get; } = new();

        // Construct from string
        public YamlParser(string text, IYamlSerializerResolver resolver)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var ms = new MemoryStream(bytes);
            _reader = new StreamReader(ms, Encoding.UTF8, leaveOpen: false);
            _resolver = resolver;
        }

        // Construct from stream
        public YamlParser(Stream stream, IYamlSerializerResolver resolver, Encoding? encoding = null)
        {
            _reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen: true);
            _resolver = resolver;
        }

        public IEnumerable<Scope> Parse()
        {
            while (ReadNextLine())
            {
                if (string.IsNullOrWhiteSpace(_currentLine)) continue;

                int indent = CountIndent(_currentLine);
                string trimmed = _currentLine.Trim();

                if (trimmed.StartsWith("!") && trimmed != "!!null")
                {
                    string tag = trimmed;
                    if (!ReadNextLine())
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not followed by a value");

                    int nextIndent = CountIndent(_currentLine);
                    if (nextIndent != indent)
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not aligned with following value");

                    if (_currentLine.TrimStart().StartsWith("- "))
                        yield return ParseSequence(indent, tag);
                    else
                        yield return ParseMapping(indent, tag);

                    continue;
                }

                if (trimmed.StartsWith("- "))
                {
                    yield return ParseSequence(indent, "");
                }
                else
                {
                    yield return ParseMapping(indent, "");
                }
            }
        }

        private MappingScope ParseMapping(int indent, string tag)
        {
            var map = new MappingScope(indent, _resolver, IdentifiableResolver, tag);
            while (!_eof)
            {
                if (string.IsNullOrWhiteSpace(_currentLine)) { if (!ReadNextLine()) break; continue; }

                int lineIndent = CountIndent(_currentLine);
                if (lineIndent < indent) break;

                string trimmed = _currentLine.Trim();
                if (trimmed.StartsWith("- ")) break;
                if (trimmed.StartsWith("!") && trimmed != "!!null")
                    throw new InvalidOperationException($"Standalone tag inside mapping is invalid: '{trimmed}'");

                var parts = trimmed.Split(':', 2);
                if (parts.Length != 2) throw new InvalidOperationException($"Invalid mapping line: '{trimmed}'");

                var key = parts[0].Trim();
                var val = parts[1].Trim();
                ReadNextLine();

                string childTag = "";
                if (val.StartsWith("!") && val != "!!null")
                {
                    var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    val = segs.Length > 1 ? segs[1].Trim() : "";
                }

                if (val.Length > 0)
                {
                    if (IsQuoted(val))
                        map.Add(key, new ScalarScope(Unquote(val), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (val.StartsWith("|"))
                        map.Add(key, new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (val.StartsWith("{") && val.EndsWith("}"))
                        map.Add(key, ParseFlowMapping(val, indent + 2, childTag));
                    else if (val.StartsWith("[") && val.EndsWith("]"))
                        map.Add(key, ParseFlowSequence(val, indent + 2, childTag));
                    else
                        map.Add(key, new ScalarScope(val, indent + 2, _resolver, IdentifiableResolver, childTag));
                }
                else
                {
                    if (!_eof)
                    {
                        int nextIndent = CountIndent(_currentLine);
                        if (nextIndent > indent)
                        {
                            var nextTrim = _currentLine.TrimStart();
                            if (nextTrim.StartsWith("- "))
                                map.Add(key, ParseSequence(indent + 2, childTag));
                            else
                                map.Add(key, ParseMapping(indent + 2, childTag));
                            continue;
                        }
                    }
                    map.Add(key, new ScalarScope(string.Empty, indent + 2, _resolver, IdentifiableResolver, childTag));
                }
            }
            return map;
        }

        private MappingScope ParseFlowMapping(string text, int indent, string tag)
        {
            var map = new MappingScope(indent, _resolver, IdentifiableResolver, tag);
            var inner = text.Substring(1, text.Length - 2).Trim();
            if (inner.Length == 0) return map;

            var entries = inner.Split(',')
                               .Select(e => e.Trim())
                               .Where(e => e.Length > 0);

            foreach (var entry in entries)
            {
                var kv = entry.Split(':', 2);
                if (kv.Length != 2)
                    throw new InvalidOperationException($"Invalid inline mapping entry: '{entry}'");

                var k = kv[0].Trim();
                var v = kv[1].Trim();

                string childTag = "";
                if (v.StartsWith("!") && v != "!!null")
                {
                    var segs = v.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    v = segs.Length > 1 ? segs[1].Trim() : "";
                }

                if (IsQuoted(v))
                    map.Add(k, new ScalarScope(Unquote(v), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (v.StartsWith("|"))
                    map.Add(k, new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (v.StartsWith("{") && v.EndsWith("}"))
                    map.Add(k, ParseFlowMapping(v, indent + 2, childTag));
                else if (v.StartsWith("[") && v.EndsWith("]"))
                    map.Add(k, ParseFlowSequence(v, indent + 2, childTag));
                else
                    map.Add(k, new ScalarScope(v, indent + 2, _resolver, IdentifiableResolver, childTag));
            }
            return map;
        }

        private SequenceScope ParseSequence(int indent, string tag)
        {
            var seq = new SequenceScope(indent, _resolver, IdentifiableResolver, tag);
            while (!_eof)
            {
                if (string.IsNullOrWhiteSpace(_currentLine)) { if (!ReadNextLine()) break; continue; }

                int lineIndent = CountIndent(_currentLine);
                if (lineIndent < indent) break;
                if (lineIndent != indent) break;

                string trimmed = _currentLine.Trim();
                if (!trimmed.StartsWith("- ")) break;

                var item = trimmed.Substring(2).Trim();
                ReadNextLine();

                string childTag = "";
                if (item.StartsWith("!") && item != "!!null")
                {
                    var segs = item.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    item = segs.Length > 1 ? segs[1].Trim() : "";
                }

                if (item.Length > 0)
                {
                    if (IsQuoted(item))
                        seq.Add(new ScalarScope(Unquote(item), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (item.StartsWith("|"))
                        seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                    else if (item.StartsWith("{") && item.EndsWith("}"))
                        seq.Add(ParseFlowMapping(item, indent + 2, childTag));
                    else if (item.StartsWith("[") && item.EndsWith("]"))
                        seq.Add(ParseFlowSequence(item, indent + 2, childTag));
                    else
                        seq.Add(new ScalarScope(item, indent + 2, _resolver, IdentifiableResolver, childTag));
                }
                else
                {
                    if (!_eof)
                    {
                        int nextIndent = CountIndent(_currentLine);
                        if (nextIndent > indent)
                        {
                            var nextTrim = _currentLine.TrimStart();
                            if (nextTrim.StartsWith("- "))
                                seq.Add(ParseSequence(indent + 2, childTag));
                            else
                                seq.Add(ParseMapping(indent + 2, childTag));
                            continue;
                        }
                    }
                    seq.Add(new ScalarScope(string.Empty, indent + 2, _resolver, IdentifiableResolver, childTag));
                }
            }
            return seq;
        }

        private SequenceScope ParseFlowSequence(string text, int indent, string tag)
        {
            var seq = new SequenceScope(indent, _resolver, IdentifiableResolver, tag);
            var inner = text.Substring(1, text.Length - 2).Trim();
            if (inner.Length == 0) return seq;

            var items = inner.Split(',')
                             .Select(e => e.Trim())
                             .Where(e => e.Length > 0);

            foreach (var raw in items)
            {
                string childTag = "";
                string value = raw;

                if (value.StartsWith("!") && value != "!!null")
                {
                    var segs = value.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    value = segs.Length > 1 ? segs[1].Trim() : "";
                }
                if (IsQuoted(value))
                    seq.Add(new ScalarScope(Unquote(value), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (value.StartsWith("|"))
                    seq.Add(new ScalarScope(ParseLiteralScalar(indent + 2), indent + 2, _resolver, IdentifiableResolver, childTag));
                else if (value.StartsWith("{") && value.EndsWith("}"))
                    seq.Add(ParseFlowMapping(value, indent + 2, childTag));
                else if (value.StartsWith("[") && value.EndsWith("]"))
                    seq.Add(ParseFlowSequence(value, indent + 2, childTag));
                else
                    seq.Add(new ScalarScope(value, indent + 2, _resolver, IdentifiableResolver, childTag));
            }
            return seq;
        }

        private string ParseLiteralScalar(int indent)
        {
            var sb = new StringBuilder();
            // consume the line with the '|' indicator
            ReadNextLine();

            while (!_eof)
            {
                if (string.IsNullOrEmpty(_currentLine)) { sb.AppendLine(); if (!ReadNextLine()) break; continue; }

                int lineIndent = CountIndent(_currentLine);
                if (lineIndent < indent) break;

                // strip the indent and append
                sb.AppendLine(_currentLine.Substring(indent));
                if (!ReadNextLine()) break;
            }

            return sb.ToString();
        }

        private bool ReadNextLine()
        {
            if (_reader.EndOfStream) { _eof = true; return false; }
            _currentLine = _reader.ReadLine();
            if (_currentLine == null) { _eof = true; return false; }
            return true;
        }

        private static bool IsQuoted(string s)
        {
            return (s.Length >= 2 &&
                   ((s.StartsWith("\"") && s.EndsWith("\"")) ||
                    (s.StartsWith("'") && s.EndsWith("'"))));
        }

        private static string Unquote(string s)
        {
            if (IsQuoted(s))
                return s.Substring(1, s.Length - 2);
            return s;
        }

        private static int CountIndent(string line)
            => line.TakeWhile(c => c == ' ').Count();

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}


