using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NexYaml.Core;
using Stride.Core.Shaders.Grammar;

namespace NexYaml.Parser.States;

internal abstract class State(YamlParser parser)
{
    public abstract NextState Parse(Utf8YamlTokenizer tokenizer);

    protected TokenType CurrentTokenType(Utf8YamlTokenizer tokenizer)
    {
        return tokenizer.CurrentTokenType;
    }
    protected Marker CurrentMark(Utf8YamlTokenizer tokenizer)
    {
        return tokenizer.CurrentMark;
    }
    protected void ThrowIfCurrentTokenUnless(Utf8YamlTokenizer tokenizer,TokenType expectedTokenType)
    {
        if (tokenizer.CurrentTokenType != expectedTokenType)
        {
            throw new YamlException(tokenizer.CurrentMark,
                $"Did not find expected token: '{expectedTokenType}'");
        }
    }
    protected void ProcessDirectives(Utf8YamlTokenizer tokenizer)
    {
        while (true)
        {
            switch (tokenizer.CurrentTokenType)
            {
                case TokenType.VersionDirective:
                    // TODO:
                    break;
                case TokenType.TagDirective:
                    // TODO:
                    break;
                default:
                    return;
            }
            tokenizer.Read();
        }
    }
    protected void PushState(ParseState state)
    {
        parser.PushState(state);
    }
}
