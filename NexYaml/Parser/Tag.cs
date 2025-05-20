namespace NexYaml.Parser;

public record Tag(string Prefix, string Handle) : ITokenContent
{
    public override string ToString()
    {
        return $"{Prefix}{Handle}";
    }
}