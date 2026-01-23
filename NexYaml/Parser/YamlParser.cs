using System.Text;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Serialization;

namespace NexYaml
{
    public sealed class YamlParser
    {
        private readonly ValueScopeFactory ValueScope = new ValueScopeFactory();
        private readonly IYamlSerializerResolver _resolver;
        private IdentifiableResolver IdentifiableResolver { get; } = new();
        private YamlReader _reader;
        public YamlParser(string text, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = new StringReader(text),
            };
        }

        public YamlParser(Stream stream, IYamlSerializerResolver resolver, Encoding? encoding = null)
        {
            var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen: true);
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = reader
            };
        }
        public YamlParser(TextReader stream, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = stream
            };
        }
        public IEnumerable<Scope> Parse()
        {
            var context = new ScopeContext(_reader, _resolver, IdentifiableResolver);
            while (_reader.Peek(out var currentLine))
            {
                if (string.IsNullOrWhiteSpace(currentLine)) continue;

                int indent = CountIndent(currentLine);
                string trimmed = currentLine.Trim();

                // Tagged root
                if (trimmed.StartsWith('!') && trimmed != YamlCodes.Null)
                {
                    _reader.Move(out var scope);
                    int spaceIndex = trimmed.IndexOf(' ');
                    string tag = spaceIndex > 0 ? trimmed.Substring(0, spaceIndex) : trimmed;
                    string inline = spaceIndex > 0 ? trimmed.Substring(spaceIndex + 1).Trim() : string.Empty;

                    if (inline.Length > 0)
                    {
                        yield return ScalarScope.Parse(context, inline, indent, tag);
                        continue;
                    }

                    if (!_reader.Peek(out var nextLine))
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not followed by a value");


                    int nextIndent = CountIndent(nextLine);
                    if (nextIndent != indent)
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not aligned with following value");

                    if (nextLine.TrimStart().StartsWith('-'))
                    {
                        yield return SequenceScope.Parse(context, indent, tag);
                    }
                    else if (nextLine.Contains(':'))
                    {
                        yield return MappingScope.Parse(context, indent, tag);
                    }
                    else
                    {
                        yield return ScalarScope.Parse(context, indent, tag);
                    }

                    continue;
                }

                // Sequence root
                if (trimmed.StartsWith('-'))
                {
                    yield return MappingScope.Parse(context, indent, string.Empty);
                }
                // Mapping root
                else if (trimmed.Contains(':'))
                {
                    yield return MappingScope.Parse(context, indent, string.Empty);
                }
                // Scalar root
                else
                {
                    yield return ScalarScope.Parse(context, indent, string.Empty);
                }
            }
        }
        private static int CountIndent(string line)
        {
            var span = line.AsSpan();
            int i = 0;
            while (i < span.Length && span[i] == ' ')
                i++;
            return i;
        }
    }
}
