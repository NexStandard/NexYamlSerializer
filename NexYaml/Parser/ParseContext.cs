using Stride.Core;

namespace NexYaml.Parser;

public struct ParseContext
{
    public ParseContext()
    {
    }

    public Guid Reference;
    public bool IsReference;
    public DataMemberMode DataMemberMode = DataMemberMode.Assign;
    public object? Value;
}