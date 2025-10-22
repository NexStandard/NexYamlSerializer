using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;

namespace NexYaml.Parser.States;

internal class FlowMapKey(YamlParser parser) : State(parser)
{
    public override NextState Parse(Utf8YamlTokenizer tokenizer)
    {
        if (tokenizer.CurrentTokenType == TokenType.FlowMappingEnd)
        {
            tokenizer.Read();
            return new NextState(ParseEventType.MappingEnd,parser.Pop());
        }

        // TODO: Why is this different to the other firstkeys??
        if (tokenizer.CurrentTokenType == TokenType.FlowEntryStart)
        {
            tokenizer.Read();
        }
        else
        {
            throw new YamlException(tokenizer.CurrentMark,
                "While parsing a flow mapping, did not find expected ',' or '}'");
        }

        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.KeyStart:
                tokenizer.Read();
                if (tokenizer.CurrentTokenType is
                    TokenType.ValueStart or
                    TokenType.FlowEntryStart or
                    TokenType.FlowMappingEnd)
                {
                    return new NextState(ParseEventType.Scalar, ParseState.FlowMappingValue);
                }
                PushState(ParseState.FlowMappingValue);
                return parser.ParseNode(false, false);
            case TokenType.ValueStart:
                return new NextState(ParseEventType.Scalar, ParseState.FlowMappingValue);
            case TokenType.FlowMappingEnd:
                tokenizer.Read();
                return new NextState(ParseEventType.MappingEnd, parser.Pop());
            default:
                PushState(ParseState.End);
                return parser.ParseNode(false, false);
        }
    }
}
