using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace NexYaml.Parser.States
{
    internal class FlowSequenceFirstEntry(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            tokenizer.Read();
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.FlowSequenceEnd:
                    tokenizer.Read();
                    return new(ParseEventType.SequenceEnd, parser.Pop());
            }

            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.FlowSequenceEnd:
                    tokenizer.Read();
                    return new(ParseEventType.SequenceEnd, parser.Pop());
                case TokenType.KeyStart:
                    tokenizer.Read();
                    return new(ParseEventType.MappingStart, ParseState.FlowSequenceEntryMappingKey);
                default:
                    PushState(ParseState.FlowSequenceEntry);
                    return parser.ParseNode(false, false);
            }
        }
    }
}
