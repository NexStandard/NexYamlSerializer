using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Shaders.Grammar;

namespace NexYaml.Parser.States
{
    internal class ParseDocumentStartImplicit(YamlParser parser) : State(parser)
    {
        public override NextState Parse(Utf8YamlTokenizer tokenizer)
        {
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.StreamEnd:
                    tokenizer.Read();
                    return new(ParseEventType.StreamEnd, ParseState.End);
                case TokenType.VersionDirective:
                case TokenType.TagDirective:
                case TokenType.DocumentStart:
                    return ParseExplicitDocumentStart(tokenizer);
                default:
                    ProcessDirectives(tokenizer);
                    PushState(ParseState.DocumentEnd);
                    return new(ParseEventType.DocumentStart, ParseState.BlockNode);
            }
        }
        private NextState ParseExplicitDocumentStart(Utf8YamlTokenizer tokenizer)
        {
            ProcessDirectives(tokenizer);
            ThrowIfCurrentTokenUnless(tokenizer,TokenType.DocumentStart);
            PushState(ParseState.DocumentEnd);
            tokenizer.Read();
            return new(ParseEventType.DocumentStart,ParseState.DocumentContent);
        }
    }
}
