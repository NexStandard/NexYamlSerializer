namespace NexYaml.Parser;

public record class Tag(string Prefix, string Handle) : ITokenContent
{
    public string Prefix { get; } = Prefix;
    public string Handle { get; } = Handle;

    public override string ToString()
    {
        return $"{Prefix}{Handle}";
    }

    public bool Equals(string tagString)
    {
        if (tagString.Length != Prefix.Length + Handle.Length)
        {
            return false;
        }
        var handleIndex = tagString.IndexOf(Prefix, StringComparison.Ordinal);
        if (handleIndex < 0)
        {
            return false;
        }
        return tagString.IndexOf(Handle, handleIndex, StringComparison.Ordinal) >= 0;
    }
}

