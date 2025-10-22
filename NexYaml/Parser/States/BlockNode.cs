using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;

namespace NexYaml.Parser.States
{
    internal class BlockNode(YamlParser parser, bool block, bool indentlessSequence) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            Anchor? anchor = null;
            Tag? Tag = null;
            ParseEventType type = ParseEventType.Scalar;
            ParseState state = ParseState.StreamStart;
            Scalar? scalar = null;

            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.Alias:
                    var state1 = parser.Pop();

                    var name = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();  // TODO: Avoid `ToString`
                    tokenizer.Read();

                    if (parser.TryResolveAnchor(name, out var anch))
                    {
                        return new(ParseEventType.Alias, state1, null, null, anch);
                    }
                    throw new YamlException(tokenizer.CurrentMark, "While parsing node, found unknown anchor");

                case TokenType.Anchor:
                    {
                        var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString(); // TODO: Avoid `ToString`
                        var anchorId = parser.PushAnchor(anchorName);
                        anchor = new Anchor(anchorName, anchorId);
                        tokenizer.Read();
                        if (tokenizer.CurrentTokenType == TokenType.Tag)
                        {
                            Tag = tokenizer.TakeCurrentTokenContent<Tag>();
                            tokenizer.Read();
                        }
                        break;
                    }
                case TokenType.Tag:
                    {
                        Tag = tokenizer.TakeCurrentTokenContent<Tag>();
                        tokenizer.Read();
                        if (tokenizer.CurrentTokenType == TokenType.Anchor)
                        {
                            tokenizer.Read();
                            var anchorName = tokenizer.TakeCurrentTokenContent<Scalar>().ToString();
                            var anchorId = parser.PushAnchor(anchorName);
                            anchor = new Anchor(anchorName, anchorId);
                        }
                        break;
                    }
            }

            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.BlockEntryStart when indentlessSequence:
                    state = ParseState.IndentlessSequenceEntry;
                    type = ParseEventType.SequenceStart;
                    break;

                case TokenType.PlainScalar:
                case TokenType.FoldedScalar:
                case TokenType.LiteralScalar:
                case TokenType.SingleQuotedScaler:
                case TokenType.DoubleQuotedScaler:
                    state = parser.Pop();
                    type = ParseEventType.Scalar;
                    scalar = tokenizer.TakeCurrentTokenContent<Scalar>();
                    tokenizer.Read();
                    break;

                case TokenType.FlowSequenceStart:
                    state = ParseState.FlowSequenceFirstEntry;
                    type = ParseEventType.SequenceStart;
                    break;

                case TokenType.FlowMappingStart:
                    state = ParseState.FlowMappingFirstKey;
                    type = ParseEventType.MappingStart;
                    break;

                case TokenType.BlockSequenceStart when block:
                    state = ParseState.BlockSequenceFirstEntry;
                    type = ParseEventType.SequenceStart;
                    break;

                case TokenType.BlockMappingStart when block:
                    state = ParseState.BlockMappingFirstKey;
                    type = ParseEventType.MappingStart;
                    break;

                // ex 7.2, an empty scalar can follow a secondary tag
                case var _ when anchor != null || Tag != null:
                    state = parser.Pop();
                    scalar = null;
                    type = ParseEventType.Scalar;
                    break;

                // consider empty entry in sequence ("- ") as null
                case TokenType.BlockEntryStart when state == ParseState.IndentlessSequenceEntry:
                    state = parser.Pop();
                    scalar = null;
                    type = ParseEventType.Scalar;
                    break;

                default:
                    {
                        throw new YamlTokenizerException(tokenizer.CurrentMark,
                            "while parsing a node, did not find expected node content");
                    }
            }
            return new NextState(type, state, scalar, Tag, anchor);
        }
    }
}
