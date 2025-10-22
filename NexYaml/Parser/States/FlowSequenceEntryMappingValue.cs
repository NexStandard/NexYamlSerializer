using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class FlowSequenceEntryMappingValue(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            if (tokenizer.CurrentTokenType == TokenType.ValueStart)
            {
                tokenizer.Read();
                if (tokenizer.CurrentTokenType is
                    TokenType.FlowEntryStart or
                    TokenType.FlowSequenceEnd)
                {
                    return new(ParseEventType.Scalar,ParseState.FlowSequenceEntryMappingEnd);
                }
                else
                {
                    PushState(ParseState.FlowSequenceEntryMappingEnd);
                    return parser.ParseNode(false, false);
                }
            }
            else
            {
                return new(ParseEventType.Scalar, ParseState.FlowSequenceEntryMappingEnd);
            }
        }
    }
}
