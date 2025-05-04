namespace NexYaml.Core;
/// <summary>
/// Manages indentation levels for YAML or similar text formatting.
/// </summary>
public class IndentationManager
{
    /// <summary>
    /// Gets the current level of indentation.
    /// </summary>
    public int CurrentIndentLevel { get; private set; }

    /// <summary>
    /// Gets or sets the number of spaces used per indentation level.
    /// </summary>
    public int IndentWidth { get; init; } = 2;

    /// <summary>
    /// Increases the current indentation level by one.
    /// </summary>
    public void IncreaseIndent()
    {
        CurrentIndentLevel++;
    }

    /// <summary>
    /// Decreases the current indentation level by one, if greater than zero.
    /// </summary>
    public void DecreaseIndent()
    {
        if (CurrentIndentLevel > 0)
        {
            CurrentIndentLevel--;
        }
    }
}