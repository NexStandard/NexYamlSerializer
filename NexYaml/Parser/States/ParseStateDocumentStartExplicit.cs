using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class ParseStateDocumentStartExplicit(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {

                while (tokenizer.CurrentTokenType == TokenType.DocumentEnd)
                {
                    tokenizer.Read();
                }


            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.StreamEnd:
                    tokenizer.Read();
                    return new(ParseEventType.StreamEnd, ParseState.End);
                default:
                    return ParseExplicitDocumentStart(tokenizer);
            }

        }
        private NextState ParseExplicitDocumentStart(Utf8YamlTokenizer tokenizer)
        {
            ProcessDirectives(tokenizer);
            ThrowIfCurrentTokenUnless(tokenizer, TokenType.DocumentStart);
            PushState(ParseState.DocumentEnd);
            tokenizer.Read();
            return new(ParseEventType.DocumentStart, ParseState.DocumentContent);
        }
    }
}
