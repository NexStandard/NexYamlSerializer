using Stride.Core;

namespace NexYaml.Core;

/// <summary>
/// Enforces <see cref="DataStyle.Compact"/> 
/// </summary>
public class StyleEnforcer
{
    private int count;
    public void Begin(ref DataStyle style)
    {
        if (style is DataStyle.Compact || count > 0)
        {
            style = DataStyle.Compact;
            count++;
        }
    }
    public void End()
    {
        if (count > 0)
        {
            count--;
        }
    }
}
