using System.Text;
using NexYaml.Core;
using NexYaml.Core.Parser;
using NexYaml.Parser;
using NexYaml.Parser.Scopes;
using NexYaml.Serialization;
using Stride.Core.Shaders.Ast;

namespace NexYaml
{
    public ref struct NewYamlParser
    {
        private readonly IYamlSerializerResolver _resolver;
        private readonly IdentifiableResolver _identifiableResolver = new();
        private YamlReader _reader;
        private ScopeContext _context;
        public NewYamlParser(string text, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = new StringReader(text),
            };
            _context = new ScopeContext(_reader, _resolver, _identifiableResolver);
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
            _context = new ScopeContext(_reader, _resolver, _identifiableResolver);
        }
        public NewYamlParser(TextReader stream, IYamlSerializerResolver resolver)
        {
            _resolver = resolver;
            _reader = new YamlReader()
            {
                Reader = stream
            };
            _context = new ScopeContext(_reader, _resolver, _identifiableResolver);
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
                            Current = new Element()
                            {
                                Data = ParsingScope.NewScalar(ScopeUtils.ParseLiteralScalar(_context, indent, inline[1]), indent, _context),
                                Tag = tag
                            };
                            return true;
                        }
                        if (inline.StartsWith('{') && inline.EndsWith('}'))
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewFlowMapping(inline, indent, _context),
                                Tag = tag
                            };
                            return true;
                        }
                        if (inline.StartsWith('[') && inline.EndsWith(']'))
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewFlowSequence(inline, indent, _context),
                                Tag = tag
                            };
                            return true;
                        }
                        else
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewScalar(inline, indent, _context),
                                Tag = tag
                            };
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
                        Current = new Element()
                        {
                            Data = ParsingScope.NewBlockSequence(indent, _context),
                            Tag = tag
                        };
                    }
                    else if (nextLine.Contains(':'))
                    {
                        Current = new Element()
                        {
                            Data = ParsingScope.NewBlockMapping(indent, _context),
                            Tag = tag
                        };
                        return true;
                    }
                    else
                    {
                        if (_context.Reader.Move(out var val))
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewScalar(val.Trim(), indent, _context),
                                Tag = tag,
                            };
                            return true;
                        }
                        throw new EndOfStreamException();
                    }
                    return true;
                }

                // Sequence root
                if (trimmed.StartsWith('-'))
                {
                    Current = new Element()
                    {
                        Data = ParsingScope.NewBlockMapping(indent, _context),
                        Tag = []
                    };
                    return true;
                }
                // Mapping root
                else if (trimmed.Contains(':'))
                {
                    Current = new Element()
                    {
                        Data = ParsingScope.NewBlockMapping(indent, _context),
                        Tag = []
                    };
                    return true;
                }
                // Scalar root
                else
                {
                    if (_context.Reader.Move(out var val))
                    {
                        if (val.SequenceEqual(YamlCodes.Null.AsSpan()))
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewNullScalar(),
                                Tag = []
                            };
                            return true;
                        }
                        else
                        {
                            Current = new Element()
                            {
                                Data = ParsingScope.NewScalar(val.Trim(), indent, _context),
                                Tag = []
                            };
                            return true;
                        }
                    }
                    throw new EndOfStreamException();
                }
            }
                return false;
        }
        public Element Current { get; private set; }
    }
}
