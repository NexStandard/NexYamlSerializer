namespace NexYaml.Core;
public class IndentationManager
{
    public int CurrentIndentLevel { get; private set; }
    public int IndentWidth { get; init; } = 2;
    public void IncreaseIndent()
    {
        CurrentIndentLevel++;
    }

    public void DecreaseIndent()
    {
        if (CurrentIndentLevel > 0)
            CurrentIndentLevel--;
    }
}
