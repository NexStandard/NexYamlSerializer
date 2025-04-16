using Stride.Core;

namespace NexYaml.Core;

/// <summary>
/// Enforces the use of <see cref="DataStyle.Compact"/> by modifying the style during a specific scope.
/// This class ensures that the style is set to compact when needed and resets it when the scope ends.
/// </summary>
public class StyleEnforcer
{
    private int count;

    /// <summary>
    /// Begins the enforcement of the <see cref="DataStyle.Compact"/> style. If the current style is not already
    /// <see cref="DataStyle.Compact"/> or the enforcement count is greater than zero, the style is set to <see cref="DataStyle.Compact"/>.
    /// </summary>
    /// <param name="style">The current <see cref="DataStyle"/> to be modified if needed.</param>
    public void Begin(ref DataStyle style)
    {
        if (style is DataStyle.Compact || count > 0)
        {
            style = DataStyle.Compact;
            count++;
        }
    }

    /// <summary>
    /// Ends the enforcement of the <see cref="DataStyle.Compact"/> style. Decrements the enforcement count.
    /// If the count reaches zero, the enforcement is no longer active.
    /// </summary>
    public void End()
    {
        if (count > 0)
        {
            count--;
        }
    }
}