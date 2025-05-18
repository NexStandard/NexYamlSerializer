using Stride.Core;

namespace NexYaml.Parser;

public ref struct ParseResult
{
    public ParseResult()
    {

    }
    public Guid Reference;
    public bool IsReference;
    public DataMemberMode DataMemberMode = DataMemberMode.Assign;
}
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