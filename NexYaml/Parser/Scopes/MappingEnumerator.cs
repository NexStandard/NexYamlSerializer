using NexYaml.Core;
using NexYaml.Core.Parser;
using SharpFont;
using Stride.Engine;
using Stride.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NexYaml.Parser.Scopes;
public class FlowMapStrategy(string[] value) : MapStrategy
{
    private int count;
    public override bool MoveNext(out Map map, Scope data)
    {
        for (var i = count; i < value.Length; i++)
        {
            count = i + 1;
            string entry = value[i];
            var kv = entry.Split(':', 2);
            if (kv.Length != 2)
                throw new InvalidOperationException($"Invalid inline mapping entry: '{entry}'");

            var key = kv[0].Trim();
            var val = kv[1].Trim();

            ReadOnlySpan<char> childTag = string.Empty;
            if (val.StartsWith('!') && !val.SequenceEqual(YamlCodes.Null))
            {
                var segs = val.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                childTag = segs[0];
                val = segs.Length > 1 ? segs[1].Trim() : string.Empty;
            }

            if (val.StartsWith('|'))
            {
                map = new Map()
                {
                    Key = key,
                    Value = ParsingScope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, val[1]), data.Indent + 2, data.Context),
                    Tag = childTag
                };
            }
            else if (val.StartsWith('{') && val.EndsWith('}'))
            {
                map = new Map()
                {
                    Value = ParsingScope.NewFlowMapping(val, data.Indent + 2, data.Context),
                    Key = key,
                    Tag = childTag
                };
            }
            else if (val.StartsWith('[') && val.EndsWith(']'))
            {
                map = new Map()
                {
                    Value = ParsingScope.NewFlowSequence(val, data.Indent + 2, data.Context),
                    Key = key,
                    Tag = childTag
                };

            }
            else if (val.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                map = new Map()
                {
                    Value = ParsingScope.NewNullScalar(),
                    Key = key
                };
            }
            else
            {
                map = new Map()
                {
                    Value = ParsingScope.NewScalar(val, data.Indent + 2, data.Context),
                    Key = key,
                    Tag = childTag
                };
            }

            return true;
        }
        map = default;
        return false;
    }
}
public class PrefixedBlockMapStrategy(string value2, string prefix) : MapStrategy
{
    bool processedPrefix = false;
    int loopIndent = 0;
    public override bool MoveNext(out Map map, Scope data)
    {
        // If we were seeded with a key (from "- key:" or "- key: value")

        if (!string.IsNullOrEmpty(value2) && !processedPrefix)
        {

            processedPrefix = true;
            ReadOnlySpan<char> childTag = [];
            ReadOnlySpan<char> valSpan = value2.AsSpan();

            if (valSpan.StartsWith('!') && !valSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                int spaceIdx = valSpan.IndexOf(' ');
                if (spaceIdx >= 0)
                {
                    childTag = valSpan.Slice(0, spaceIdx);
                    valSpan = valSpan.Slice(spaceIdx + 1).Trim();
                }
                else
                {
                    childTag = valSpan;
                    valSpan = ReadOnlySpan<char>.Empty;
                }
            }
            ReadOnlySpan<char> val = valSpan;
            ReadOnlySpan<char> t = childTag;
            data.Context.HandleMapping(data.Indent,val, prefix, t, out map);
            return true;
        }
        else if (!processedPrefix)
        {
            data.Context.Reader.Move();
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
                        map = new Map()
                        {
                            Value = ParsingScope.NewBlockSequence(data.Indent + 2, data.Context),
                            Key = prefix,
                            Tag = []
                        };
                    }
                    else
                    {
                        map = new Map()
                        {
                            Value = ParsingScope.NewBlockMapping(data.Indent + 2, data.Context),
                            Key = prefix,
                            Tag = []
                        };
                    }
                    return true;
                }
                map = new Map()
                {
                    Value = ParsingScope.NewScalar(string.Empty, data.Indent + 2, data.Context),
                    Key = prefix,
                    Tag = []
                };
                return true;

            }

        }
        while (data.Context.Reader.Peek(out var next))
        {
            // Work with spans to avoid allocations
            ReadOnlySpan<char> line = next;

            int lineIndent = ScopeUtils.CountIndent(next);
            if (lineIndent < data.Indent || lineIndent < loopIndent)
            {
                map = default;
                return false;
            }
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
                map = default;
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


            ReadOnlySpan<char> itemSpan = valSpan.Trim();
            ReadOnlySpan<char> key = keySpan.Trim();
            // Inline tag handling
            ReadOnlySpan<char> childTag = [];
            if (!itemSpan.IsEmpty && itemSpan[0] == '!' && !itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                int spaceIdx = itemSpan.IndexOf(' ');
                if (spaceIdx >= 0)
                {
                    childTag = itemSpan[..spaceIdx];
                    itemSpan = itemSpan[(spaceIdx + 1)..].Trim();
                }
                else
                {
                    childTag = itemSpan;
                    itemSpan = ReadOnlySpan<char>.Empty;
                }
            }
            else
            {
                childTag = ReadOnlySpan<char>.Empty;
            }


            if (itemSpan.Length > 0)
            {
                ReadOnlySpan<char> t = childTag;
                data.Context.HandleMapping(data.Indent, itemSpan, key, t, out map);
                return true;
            }
            else
            {
                data.Context.Reader.Move();
                // Look ahead for nested structures
                if (data.Context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = ScopeUtils.CountIndent(lookahead);
                    ReadOnlySpan<char> nextTrim = lookahead[nextIndent..].Trim();

                    if (nextIndent > data.Indent)
                    {
                        if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                        {
                            map = new Map()
                            {
                                Value = ParsingScope.NewBlockSequence(data.Indent + 2, data.Context),
                                Key = key,
                                Tag = childTag
                            };
                        }
                        else
                        {
                            map = new Map()
                            {
                                Value = ParsingScope.NewBlockMapping(data.Indent + 2, data.Context),
                                Key = key,
                                Tag = childTag
                            };
                        }
                        return true;
                    }
                    else if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                    {
                        map = new Map()
                        {
                            Value = ParsingScope.NewBlockSequence(data.Indent, data.Context),
                            Key = key,
                            Tag = childTag
                        };
                        return true;
                    }
                }

                // Default: empty scalar
                map = new Map()
                {
                    Value = ParsingScope.NewScalar(string.Empty, data.Indent + 2, data.Context),
                    Key = key,
                    Tag = childTag
                };
                return true;
            }
        }
        map = default;
        return false;
    }
}
public class BlockMapStrategy : MapStrategy
{
    int loopIndent = 0;
    public override bool MoveNext(out Map map, Scope data)
    {
        while (data.Context.Reader.Peek(out var next))
        {
            // Work with spans to avoid allocations
            ReadOnlySpan<char> line = next;

            int lineIndent = ScopeUtils.CountIndent(next);
            if (lineIndent < data.Indent || lineIndent < loopIndent)
            {
                map = default;
                return false;
            }
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
                map = default;
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


            ReadOnlySpan<char> itemSpan = valSpan.Trim();
            ReadOnlySpan<char> key = keySpan.Trim();
            // Inline tag handling
            ReadOnlySpan<char> childTag = [];
            if (!itemSpan.IsEmpty && itemSpan[0] == '!' && !itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                int spaceIdx = itemSpan.IndexOf(' ');
                if (spaceIdx >= 0)
                {
                    childTag = itemSpan[..spaceIdx];
                    itemSpan = itemSpan[(spaceIdx + 1)..].Trim();
                }
                else
                {
                    childTag = itemSpan;
                    itemSpan = ReadOnlySpan<char>.Empty;
                }
            }
            else
            {
                childTag = ReadOnlySpan<char>.Empty;
            }


            if (itemSpan.Length > 0)
            {
                ReadOnlySpan<char> t = childTag;
                data.Context.HandleMapping(data.Indent, itemSpan, key, t, out map);
                return true;
            }
            else
            {
                data.Context.Reader.Move();
                // Look ahead for nested structures
                if (data.Context.Reader.Peek(out var lookahead))
                {
                    int nextIndent = ScopeUtils.CountIndent(lookahead);
                    ReadOnlySpan<char> nextTrim = lookahead[nextIndent..].Trim();

                    if (nextIndent > data.Indent)
                    {
                        if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                        {
                            map = new Map()
                            {
                                Value = ParsingScope.NewBlockSequence(data.Indent + 2, data.Context),
                                Key = key,
                                Tag = childTag
                            };
                        }
                        else
                        {
                            map = new Map()
                            {
                                Value = ParsingScope.NewBlockMapping(data.Indent + 2, data.Context),
                                Key = key,
                                Tag = childTag
                            };
                        }
                        return true;
                    }
                    else if (!nextTrim.IsEmpty && nextTrim[0] == '-')
                    {
                        map = new Map()
                        {
                            Value = ParsingScope.NewBlockSequence(data.Indent, data.Context),
                            Key = key,
                            Tag = childTag
                        };
                        return true;
                    }
                }

                // Default: empty scalar
                map = new Map()
                {
                    Value = ParsingScope.NewScalar(string.Empty, data.Indent + 2, data.Context),
                    Key = key,
                    Tag = childTag
                };
                return true;
            }
        }
        map = default;
        return false;
    }

}
internal static class ScopeStrategyExtensions
{
    internal static void HandleMapping(this ScopeContext data, int Indent, ReadOnlySpan<char> val, ReadOnlySpan<char> key, ReadOnlySpan<char> childTag, out Map map)
    {
        if (val.StartsWith('|'))
        {
            data.Reader.Move();
            map = new Map()
            {
                Value = ParsingScope.NewScalar(ScopeUtils.ParseLiteralScalar(data, Indent + 1, val[1]), Indent + 2, data),
                Key = key,
                Tag = childTag
            };
        }
        else if (val.StartsWith('{') && val.EndsWith('}'))
        {
            data.Reader.Move();
            map = new Map()
            {
                Value = ParsingScope.NewFlowMapping(val, Indent + 2, data),
                Key = key,
                Tag = childTag
            };
        }
        else if (val.StartsWith('[') && val.EndsWith(']'))
        {
            data.Reader.Move();
            map = new Map()
            {
                Value = ParsingScope.NewFlowSequence(val, Indent + 2, data),
                Key = key,
                Tag = childTag
            };
        }
        else if (val.SequenceEqual(YamlCodes.Null.AsSpan()))
        {

            data.Reader.Move();
            map = new Map()
            {
                Value = ParsingScope.NewNullScalar(),
                Key = key
            };
        }
        else
        {
            map = new Map()
            {
                Value = ParsingScope.NewLazyScalar(111, Indent + 2, data),
                Key = key,
                Tag = childTag
            };
        }
    }
}
