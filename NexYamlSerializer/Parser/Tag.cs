#nullable enable
using System;

namespace NexVYaml.Parser;

public record class Tag(string prefix, string handle) : ITokenContent
{
    public string Prefix { get; } = prefix;
    public string Handle { get; } = handle;

    public override string ToString() => $"{Prefix}{Handle}";

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

