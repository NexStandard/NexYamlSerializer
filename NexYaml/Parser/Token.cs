namespace NexYaml.Parser;

internal record struct Token(TokenType type, ITokenContent? content = null)
{
    public readonly TokenType Type = type;
    public readonly ITokenContent? Content = content;
}
