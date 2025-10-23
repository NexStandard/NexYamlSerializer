using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexYaml.Parser.States
{
    internal class DocumentContent(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.VersionDirective:
                case TokenType.TagDirective:
                case TokenType.DocumentStart:
                case TokenType.DocumentEnd:
                case TokenType.StreamEnd:
                    return new(ParseEventType.Scalar,parser.Pop());
                default:
                    return new BlockNode(parser,true,false).Parse(tokenizer);
            }
        }
    }
}
