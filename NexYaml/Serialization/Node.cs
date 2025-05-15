using Stride.Core;

namespace NexYaml.Serialization;

public abstract class Node
{
    public abstract WriteContext<Mapping> BeginMapping<T>(WriteContext<T> context, string tag, DataStyle style)
        where T : Node;
    public abstract WriteContext<Sequence> BeginSequence<T>(WriteContext<T> context, string tag, DataStyle style)
        where T : Node;
    public virtual void End<T>(WriteContext<T> context) where T : Node
    {
        // standard do nothing
    }
}
