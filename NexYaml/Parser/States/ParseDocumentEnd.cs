using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class ParseDocumentEnd(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            if (CurrentTokenType(tokenizer) == TokenType.DocumentEnd)
            {
                tokenizer.Read();
            }

            // TODO tag handling // what does that mean, which tags?
            return new(ParseEventType.DocumentEnd, ParseState.DocumentStart);
        }
    }
}
