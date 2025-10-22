using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class FlowSequenceEntryMappingKey(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            if (tokenizer.CurrentTokenType is
                TokenType.ValueStart or
                TokenType.FlowEntryStart or
                TokenType.FlowSequenceEnd)
            {
                tokenizer.Read();
                return new(ParseEventType.Scalar,ParseState.FlowSequenceEntryMappingValue);
            }
            else
            {
                PushState(ParseState.FlowSequenceEntryMappingValue);
                return parser.ParseNode(false, false);
            }
        }
    }
}
