using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States;

internal class FlowMapValue(YamlParser parser) : State(parser)
{
    public override NextState Parse(Utf8YamlTokenizer tokenizer)
    {
        if (tokenizer.CurrentTokenType == TokenType.ValueStart)
        {
            tokenizer.Read();
            if (tokenizer.CurrentTokenType is not TokenType.FlowEntryStart and
                not TokenType.FlowMappingEnd)
            {
                PushState(ParseState.FlowMappingKey);
                return parser.ParseNode(false, false);
            }
        }
        return new(ParseEventType.Scalar,ParseState.FlowMappingKey);
    }
}
