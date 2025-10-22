using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;

namespace NexYaml.Parser.States;

internal class BlockSequenceEntry(YamlParser parser) : State(parser)
{
    public override NextState Parse(Utf8YamlTokenizer tokenizer)
    {
        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.BlockEnd:
                tokenizer.Read();
                return new NextState(ParseEventType.SequenceEnd, parser.Pop());
            case TokenType.BlockEntryStart:
                tokenizer.Read();
                if (tokenizer.CurrentTokenType is TokenType.BlockEntryStart or TokenType.BlockEnd)
                {
                    return new NextState(ParseEventType.Scalar, ParseState.BlockSequenceEntry);
                }

                PushState(ParseState.BlockSequenceEntry);
                return parser.ParseNode(true, false);
            default:
                throw new YamlException(tokenizer.CurrentMark,
                    "while parsing a block collection, did not find expected '-' indicator");
        }
    }
}
