using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class FlowMapFirstKey(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            tokenizer.Read();
            if (tokenizer.CurrentTokenType == TokenType.FlowMappingEnd)
            {
                tokenizer.Read();
                return new NextState(ParseEventType.MappingEnd, parser.Pop());
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
}
