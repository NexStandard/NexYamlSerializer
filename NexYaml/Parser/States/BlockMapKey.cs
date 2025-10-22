using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using NexYaml.Core;

namespace NexYaml.Parser.States;

internal class BlockMapKey(YamlParser parser) : State(parser)
{
    public override NextState Parse(Utf8YamlTokenizer tokenizer)
    {
        switch (tokenizer.CurrentTokenType)
        {
            case TokenType.KeyStart:
                tokenizer.Read();
                if (tokenizer.CurrentTokenType is
                    TokenType.KeyStart or
                    TokenType.ValueStart or
                    TokenType.BlockEnd)
                {
                    return new NextState(ParseEventType.Scalar, ParseState.BlockMappingValue, null, null, null);
                }
                else
                {
                    PushState(ParseState.BlockMappingValue);
                    return new BlockNode(parser, true, true).Parse(tokenizer);
                }
            case TokenType.ValueStart:
                return new NextState(ParseEventType.Scalar, ParseState.BlockMappingValue, null, null, null);
            case TokenType.BlockEnd:
                tokenizer.Read();
                return new NextState(ParseEventType.MappingEnd, parser.Pop(), null, null, null);
            default:
                throw new YamlException(tokenizer.CurrentMark,
                    "while parsing a block mapping, did not find expected key");
        }
    }
}
