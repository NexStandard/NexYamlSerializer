using NexYaml.Core;
using NexYaml.Serialization;
using SharpFont;
using Stride.Core.Shaders.Ast;
using Stride.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Stride.Graphics.Buffer;

namespace NexYaml.Parser.Scopes;

public class BlockSequenceStrategy() : SequenceStrategy
{
    public override bool MoveNext(out Element scope, Scope data)
    {
        while (data.Context.Reader.Peek(out var next))
        {
            int lineIndent = ScopeUtils.CountIndent(next);
            if (lineIndent != data.Indent)
            {
                scope = default;
                return false;
            }

            ReadOnlySpan<char> line = next[lineIndent..];
            if (line.Length == 0 || line[0] != '-')
            {
                scope = default;
                return false;
            }

            // consume this line


            // skip the leading '-' and any following spaces
            var itemSpan = line[1..].Trim();

            // tag detection
            ReadOnlySpan<char> childTag;
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

                switch (itemSpan[0])
                {
                    // literal block scalar
                    case '|':
                        data.Context.Reader.Move();
                        scope = new Element()
                        {
                            Data = ParsingScope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 1, itemSpan[1]), data.Indent + 2, data.Context),
                            Tag = childTag
                        };
                        return true;
                    // flow mapping
                    case '{' when itemSpan[^1] == '}':
                        data.Context.Reader.Move();
                        scope = new Element()
                        {
                            Data = ParsingScope.NewFlowMapping(itemSpan, data.Indent + 2, data.Context),
                            Tag = childTag
                        };
                        return true;
                    // flow sequence
                    case '[' when itemSpan[^1] == ']':
                        data.Context.Reader.Move();
                        scope = new Element()
                        {
                            Data = ParsingScope.NewFlowSequence(itemSpan, data.Indent + 2, data.Context),
                            Tag = childTag
                        };
                        return true;
                    // inline mapping (key: value)
                    default:
                        int colonIdx = itemSpan.IndexOf(':');
                        if (colonIdx >= 0)
                        {
                            var keySpan = itemSpan[..colonIdx].Trim();
                            var valSpan = itemSpan[(colonIdx + 1)..].Trim();
                            scope = new Element()
                            {
                                Data = ParsingScope.NewPrefixedBlockMapping(valSpan, keySpan.ToString(), data.Indent + 2, data.Context),
                                Tag = childTag
                            };
                        }
                        else
                        {
                            data.Context.Reader.Move();
                            if (itemSpan.SequenceEqual(YamlCodes.Null.AsSpan()))
                            {
                                scope = new Element()
                                {
                                    Data = ParsingScope.NewNullScalar()
                                };
                            }
                            else
                            {
                                scope = new Element()
                                {
                                    Data = ParsingScope.NewScalar(itemSpan, data.Indent + 2, data.Context),
                                    Tag = childTag
                                };
                            }
                        }
                        return true;
                }
            }
            else
            {
                data.Context.Reader.Move();
                // empty item, look ahead for nested structure
                if (data.Context.Reader.Peek(out var la))
                {
                    int nextIndent = ScopeUtils.CountIndent(la);
                    var nextTrim = la[nextIndent..];

                    if (nextIndent > data.Indent)
                    {
                        if (nextTrim.Length > 0 && nextTrim[0] == '-')
                        {
                            scope = new Element()
                            {
                                Data = ParsingScope.NewBlockSequence(data.Indent + 2, data.Context),
                                Tag = childTag
                            };

                        }
                        else
                        {
                            scope = new Element()
                            {
                                Data = ParsingScope.NewBlockMapping(data.Indent + 2, data.Context),
                                Tag = childTag
                            };
                        }
                        return true;
                    }
                }
                scope = new Element()
                {
                    Data = ParsingScope.NewScalar(string.Empty, data.Indent + 2, data.Context),
                    Tag = childTag
                };
                return true;
            }
        }
        scope = default;
        return false;
    }
}
public class FlowSequenceStrategy(string[] value) : SequenceStrategy
{
    private int count;
    string bufferedTag = string.Empty;
    public override bool MoveNext(out Element scope, Scope data)
    {

        for (var i = count; i < value.Length; i++)
        {
            count = i + 1;
            string item = value[i];

            // Tag extraction
            if (item.StartsWith('!') && item != YamlCodes.Null)
            {
                var segs = item.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (segs.Length == 1)
                {
                    bufferedTag = segs[0];
                    continue;
                }
                else
                {
                    bufferedTag = segs[0];
                    item = segs.Length > 1 ? segs[1].Trim() : string.Empty;
                }
            }

            if (item.StartsWith('|'))
            {
                scope = new Element()
                {
                    Data = ParsingScope.NewScalar(ScopeUtils.ParseLiteralScalar(data.Context, data.Indent + 2, item[1]), data.Indent + 2, data.Context),
                    Tag = bufferedTag
                };
            }
            else if (item.StartsWith('{') && item.EndsWith('}'))
            {
                scope = new Element()
                {
                    Data = ParsingScope.NewFlowMapping(item, data.Indent + 2, data.Context),
                    Tag = bufferedTag
                };
            }
            else if (item.StartsWith('[') && item.EndsWith(']'))
            {
                scope = new Element()
                {
                    Data = ParsingScope.NewFlowSequence(item, data.Indent, data.Context),
                    Tag = bufferedTag
                };
            }
            else if (item.SequenceEqual(YamlCodes.Null.AsSpan()))
            {
                scope = new Element()
                {
                    Data = ParsingScope.NewNullScalar(),
                    Tag = []
                };
            }
            else
            {
                scope = new Element()
                {
                    Data = ParsingScope.NewScalar(item, data.Indent + 2, data.Context),
                    Tag = bufferedTag
                };
            }
            return true;
        }
        scope = default;
        return false;
    }
}
