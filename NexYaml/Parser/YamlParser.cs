using System.Text;
using NexYaml.Core;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core.Shaders.Ast;

namespace NexYaml
{
    public ref struct NewYamlParser
    {
        private readonly IYamlSerializerResolver _resolver;
        private IdentifiableResolver IdentifiableResolver { get; } = new();
        private YamlReader _reader;
        private ScopeContext context;
        public NewYamlParser(string text, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = new StringReader(text),
            };
            context = new ScopeContext(_reader, _resolver, IdentifiableResolver);
        }
        public NewYamlParser GetEnumerator() => this;
        public NewYamlParser(Stream stream, IYamlSerializerResolver resolver, Encoding? encoding = null)
        {
            var reader = new StreamReader(stream, encoding ?? Encoding.UTF8, leaveOpen: true);
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = reader
            };
            context = new ScopeContext(_reader, _resolver, IdentifiableResolver);
        }
        public NewYamlParser(TextReader stream, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = stream
            };
            context = new ScopeContext(_reader, _resolver, IdentifiableResolver);
        }
        public bool MoveNext()
        {
            while (_reader.Peek(out var currentLine))
            {
                if (currentLine.Length == 0) continue;

                int indent = ScopeUtils.CountIndent(currentLine);
                ReadOnlySpan<char> trimmed = currentLine.Trim();

                // Tagged root
                if (trimmed.StartsWith('!') && !trimmed.SequenceEqual(YamlCodes.Null))
                {
                    _reader.Move(out var scope);
                    int spaceIndex = trimmed.IndexOf(' ');
                    ReadOnlySpan<char> tag = spaceIndex > 0 ? trimmed[..spaceIndex] : trimmed;
                    ReadOnlySpan<char> inline = spaceIndex > 0 ? trimmed[(spaceIndex + 1)..].Trim() : [];

                    if (inline.Length > 0)
                    {
                        if (inline.StartsWith('|'))
                        {
                            Current = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(context, indent, inline[1]), indent, context, tag);

                            return true;
                        }
                        if (inline.StartsWith('{') && inline.EndsWith('}'))
                        {
                            Current = Scope.NewFlowMapping(inline, indent, context, tag);
                            return true;
                        }
                        if (inline.StartsWith('[') && inline.EndsWith(']'))
                        {
                            Current = Scope.NewFlowSequence(inline, indent, context, tag);
                            return true;
                        }
                        else
                        {
                            Current = Scope.NewScalar(inline, indent, context, tag);
                            return true;
                        }
                    }

                    if (!_reader.Peek(out var nextLine))
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not followed by a value");


                    int nextIndent = ScopeUtils.CountIndent(nextLine);
                    if (nextIndent != indent)
                        throw new InvalidOperationException($"Tag '{tag}' at indent {indent} not aligned with following value");

                    if (nextLine.TrimStart().StartsWith('-'))
                    {
                        Current = Scope.NewBlockSequence(indent, context, tag);
                    }
                    else if (nextLine.Contains(':'))
                    {
                        Current = Scope.NewBlockMapping(indent, context, tag);
                        return true;
                    }
                    else
                    {
                        if (context.Reader.Move(out var val))
                        {
                            Current = Scope.NewScalar(val.Trim(), indent, context, tag);
                            return true;
                        }
                        throw new EndOfStreamException();
                    }
                    return true;
                }

                // Sequence root
                if (trimmed.StartsWith('-'))
                {
                    Current = Scope.NewBlockMapping(indent, context, string.Empty);
                    return true;
                }
                // Mapping root
                else if (trimmed.Contains(':'))
                {
                    Current = Scope.NewBlockMapping(indent, context, string.Empty);
                    return true;
                }
                // Scalar root
                else
                {
                    if (context.Reader.Move(out var val))
                    {
                        Current = Scope.NewScalar(val.Trim(), indent, context, string.Empty);
                        return true;
                    }
                    throw new EndOfStreamException();
                }
            }
                return false;
        }
        public Scope Current { get; private set; }
    }
    public sealed class YamlParser
    {
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

        private static int CountIndent(ReadOnlySpan<char> line)
        {
            int i = 0;
            while (i < line.Length && line[i] == ' ')
                i++;
            return i;
        }
    }
}
