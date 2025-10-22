using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States;

internal class IndentlessSequenceEntry(YamlParser parser) : State(parser)
{
    public override NextState Parse(Utf8YamlTokenizer tokenizer)
    {
        if (tokenizer.CurrentTokenType != TokenType.BlockEntryStart)
        {
            return new(ParseEventType.SequenceEnd,parser.Pop());
        }

        tokenizer.Read();

        if (tokenizer.CurrentTokenType is
            TokenType.KeyStart or
            TokenType.ValueStart or
            TokenType.BlockEnd)
        {
            return new(ParseEventType.Scalar,ParseState.IndentlessSequenceEntry);
        }
        else
        {
            PushState(ParseState.IndentlessSequenceEntry);
            return parser.ParseNode(true, false);
        }
    }
}
