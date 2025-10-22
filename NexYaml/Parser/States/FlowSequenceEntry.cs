using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;

namespace NexYaml.Parser.States
{
    internal class FlowSequenceEntry(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.FlowSequenceEnd:
                    tokenizer.Read();
                    return new(ParseEventType.SequenceEnd,parser.Pop());
                case TokenType.FlowEntryStart:
                    tokenizer.Read();
                    break;
                default:
                    throw new YamlException(tokenizer.CurrentMark, "while parsing a flow sequence, expected ',' or ']'");
            }

            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.FlowSequenceEnd:
                    tokenizer.Read();
                    return new(ParseEventType.SequenceEnd, parser.Pop());
                case TokenType.KeyStart:
                    tokenizer.Read();
                    return new(ParseEventType.MappingStart,ParseState.FlowSequenceEntryMappingKey);
                default:
                    PushState(ParseState.FlowSequenceEntry);
                    return parser.ParseNode(false, false);
            }
        }
    }
}
