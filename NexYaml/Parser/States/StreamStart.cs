using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class StreamStart(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            if (CurrentTokenType(tokenizer) == TokenType.None)
            {
                tokenizer.Read();
            }
            ThrowIfCurrentTokenUnless(tokenizer,TokenType.StreamStart);
            tokenizer.Read();
            return new NextState(ParseEventType.StreamStart, ParseState.ImplicitDocumentStart);
        }
    }
}
