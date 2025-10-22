using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class BlockMapValue(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            if (tokenizer.CurrentTokenType == TokenType.ValueStart)
            {
                tokenizer.Read();
                if (tokenizer.CurrentTokenType is
                    TokenType.KeyStart or
                    TokenType.ValueStart or
                    TokenType.BlockEnd)
                {
                    return new(ParseEventType.Scalar,ParseState.BlockMappingKey);
                }
                else
                {
                    PushState(ParseState.BlockMappingKey);
                    return parser.ParseNode(true, true);
                }
            }
            else
            {
                return new(ParseEventType.Scalar, ParseState.BlockMappingKey);
            }
        }
    }
}
