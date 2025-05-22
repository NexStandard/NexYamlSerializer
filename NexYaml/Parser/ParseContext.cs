using Stride.Core;

namespace NexYaml.Parser;

public record struct ParseContext
{
    public ParseContext()
    {
    }

    public DataMemberMode DataMemberMode = DataMemberMode.Assign;
    public object? Value;
}