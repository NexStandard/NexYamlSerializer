using System;
using System.Linq;

namespace NexYamlSerializer.Emitter;
public class IndentationManager
{
    public int CurrentIndentLevel { get; private set; }

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
