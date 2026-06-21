using NexYaml.Core;

namespace NexYaml.Parser.Scopes
{
    public ref struct MappingEnumerator
    {
        private int count;
        public Map Current { get; private set; }
        private string[] value;
        string bufferedTag = string.Empty;
        Scope data;
        private string value2;
        private string prefix;
        private bool processedPrefix = false;
        private int loopIndent;
        public MappingEnumerator GetEnumerator() => this;
        public MappingEnumerator(Scope data, string value)
        {
            this.data = data;
            this.value = ScopeUtils.NewSplitItems(value.Substring(1, value.Length - 2).Trim());
        }
        public MappingEnumerator(Scope data)
        {
            this.data = data;
        }
        public MappingEnumerator(Scope data, string value, string prefix)
        {
            this.value2 = value;
            this.prefix = prefix;
            this.data = data;
        }
        public bool MoveNext()
        {
            if (data.Kind is ScopeKind.FlowMapping)
            {
                return ParseFlowMapping();
            }
            if (data.Kind is ScopeKind.BlockMapping)
            {
                return ParseBlockMapping();
            }
            if (data.Kind is ScopeKind.PrefixedBlockMapping)
            {
                return ParsePrefixedMapping();
            }
            return false;
        }
        public bool ParseFlowMapping()
        {

            for (var i = count; i < value.Length; i++)
            {
                count = i + 1;
                string entry = this.value[i];
                var kv = entry.Split(':', 2);
                if (kv.Length != 2)
                    throw new InvalidOperationException($"Invalid inline mapping entry: '{entry}'");

                var key = kv[0].Trim();
                var val = kv[1].Trim();

                string childTag = string.Empty;
                if (val.StartsWith('!') && val != YamlCodes.Null)
                {
                    var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                    childTag = segs[0];
                    val = segs.Length > 1 ? segs[1].Trim() : string.Empty;
                }

                if (val.StartsWith('|'))
                {
                    Current = new Map()
                    {
                        Key = key,
                        Value = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, val[1]), data.Indent + 2, data.Context, childTag.ToString())
                    };
                }
                else if (val.StartsWith('{') && val.EndsWith('}'))
                {
                    Current = new Map()
                    {
                        Value = Scope.NewFlowMapping(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = key
                    };
                }
                else if (val.StartsWith('[') && val.EndsWith(']'))
                {
                    Current = new Map()
                    {
                        Value = Scope.NewFlowSequence(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = key
                    };
                    
                }
                else
                {
                    Current = new Map()
                    {
                        Value = Scope.NewScalar(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = key
                    };
                }

                return true;
            }
            return false;
        }
        public bool ParseBlockMapping()
        {
            while (data.Context.Reader.Peek(out var next))
            {
                // Work with spans to avoid allocations
                ReadOnlySpan<char> line = next;

                int lineIndent = ScopeUtils.CountIndent(next);
                if (lineIndent < data.Indent || lineIndent < loopIndent)
                    return false;
                loopIndent = lineIndent;

                // Slice off leading spaces
                ReadOnlySpan<char> trimmed = line.Slice(lineIndent);
                if (trimmed.IsEmpty)
                {
                    data.Context.Reader.Move(); continue;
                }

                // Sequence indicator means mapping ends
                if (trimmed[0] == '-')
                {
                    return false;
                }

                // Standalone tag check
                if (trimmed[0] == '!' && !trimmed.SequenceEqual(YamlCodes.Null.AsSpan()))
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
                data.Context.Reader.Move();

                ReadOnlySpan<char> val = valSpan.Trim();
                ReadOnlySpan<char> key = keySpan.Trim();
                // Inline tag handling
                ScopeUtils.ExtractTag(ref val, out var childTag);

                if (val.Length > 0)
                {
                    if (val.StartsWith('|'))
                    {
                        Current = new Map()
                        {
                            Value = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, val[1]), data.Indent + 2, data.Context, childTag),
                            Key = key
                        };
                    }
                    else if (val.StartsWith('{') && val.EndsWith('}'))
                    {
                        Current = new Map()
                        {
                            Value = Scope.NewFlowMapping(val, data.Indent + 2, data.Context, childTag),
                            Key = key
                        };
                    }
                    else if (val.StartsWith('[') && val.EndsWith(']'))
                    {
                        Current = new Map()
                        {
                            Value = Scope.NewFlowSequence(val, data.Indent + 2, data.Context, childTag),
                            Key = key
                        };
                    }
                    else
                    {
                        Current = new Map()
                        {
                            Value = Scope.NewScalar(val, data.Indent + 2, data.Context, childTag),
                            Key = key
                        };
                    }
                    return true;
                }
                else
                {
                    // Look ahead for nested structures
                    if (data.Context.Reader.Peek(out var lookahead))
                    {
                        int nextIndent = ScopeUtils.CountIndent(lookahead);
                        ReadOnlySpan<char> nextTrim = lookahead[nextIndent..].Trim();

                        if (nextIndent > data.Indent)
                        {
                            if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                            {
                                Current = new Map()
                                {
                                    Value = Scope.NewBlockSequence(data.Indent + 2, data.Context, childTag.ToString()),
                                    Key = key
                                };
                            }
                            else
                            {
                                Current = new Map()
                                {
                                    Value = Scope.NewBlockMapping(data.Indent + 2, data.Context, childTag.ToString()),
                                    Key = key
                                };
                            }
                            return true;
                        }
                        else if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                        {
                            Current = new Map()
                            {
                                Value = Scope.NewBlockSequence(data.Indent, data.Context, childTag.ToString()),
                                Key = key
                            };
                            return true;
                        }
                    }

                    // Default: empty scalar
                    Current = new Map()
                    {
                        Value = Scope.NewScalar(string.Empty, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = key
                    };
                    return true;
                }
            }
            return false;
        }
        public bool ParsePrefixedMapping()
        {
            // If we were seeded with a key (from "- key:" or "- key: value")

            if (!string.IsNullOrEmpty(value2) && !processedPrefix)
            {
                processedPrefix = true;
                string childTag = string.Empty;
                ReadOnlySpan<char> valSpan = value2.AsSpan();

                if (valSpan.StartsWith('!') && !valSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
                {
                    int spaceIdx = valSpan.IndexOf(' ');
                    if (spaceIdx >= 0)
                    {
                        childTag = valSpan.Slice(0, spaceIdx).ToString();
                        valSpan = valSpan.Slice(spaceIdx + 1).Trim();
                    }
                    else
                    {
                        childTag = valSpan.ToString();
                        valSpan = ReadOnlySpan<char>.Empty;
                    }
                }
                string val = valSpan.ToString();
                if (val.StartsWith('|'))
                {
                    Current = new Map()
                    {
                        Value = Scope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, val[1]), data.Indent + 2, data.Context, childTag.ToString()),
                        Key = prefix
                    };
                }
                else if (val.StartsWith('{') && val.EndsWith('}'))
                {
                    Current = new Map()
                    {
                        Value = Scope.NewFlowMapping(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = prefix
                    };
                }
                else if (val.StartsWith('[') && val.EndsWith(']'))
                {
                    Current = new Map()
                    {
                        Value = Scope.NewFlowSequence(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = prefix
                    };
                }
                else
                {
                    Current = new Map()
                    {
                        Value = Scope.NewScalar(val, data.Indent + 2, data.Context, childTag.ToString()),
                        Key = prefix
                    };
                }
                return true;
            }
            else if(!processedPrefix)
            {
                // No inline value: "- key:" followed by nested mapping/sequence
                processedPrefix = true;
                if (data.Context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = ScopeUtils.CountIndent(lookahead);
                    var nextTrim = lookahead[nextIndent..];

                    if (nextIndent > data.Indent)
                    {
                        if (nextTrim[0] == '-')
                        {
                            Current = new Map()
                            {
                                Value = Scope.NewBlockSequence(data.Indent + 2, data.Context, string.Empty),
                                Key = prefix
                            };
                        }
                        else
                        {
                            Current = new Map()
                            {
                                Value = Scope.NewBlockMapping(data.Indent + 2, data.Context, string.Empty),
                                Key = prefix
                            };
                        }
                        return true;
                    }
                        Current = new Map()
                        {
                            Value = Scope.NewScalar(string.Empty, data.Indent + 2, data.Context, string.Empty),
                            Key = prefix
                        };
                        return true;
                    
                }
            }
            var result = ParseBlockMapping();
            return result;
        }
    }
}

